using UnityEngine;
using System;
using System.Collections.Generic;

public class SearchZoneReceiverManager : MonoBehaviour
{
    [Header("Prefab visual para representar zonas")]
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private SearchRouteGenerator routeGenerator;

    private const string pendingZonesTopic = MQTTConstants.PendingSearchZonesTopic;
    private const string pendingRequestTopic = MQTTConstants.PendingSearchZonesRequestTopic;

    private readonly Queue<string> pendingJsonQueue = new();
    private readonly List<GameObject> instantiatedZones = new();
    private List<SearchZoneSummary> lastReceivedZones = new();

    private Action<string> cachedHandler;

    public static SearchZoneReceiverManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        cachedHandler = EnqueueJson;
        Debug.Log("📦 SearchZoneReceiverManager → Awake completado.");
    }

    private void OnEnable()
    {
        Debug.Log("🟢 SearchZoneReceiverManager habilitado.");

        if (MQTTClient.Instance != null && MQTTClient.Instance.GetClient()?.IsConnected == true)
        {
            Debug.Log("✅ Cliente MQTT ya conectado. Suscribiendo a zonas y solicitando...");
            MQTTClient.Instance.RegisterHandler(pendingZonesTopic, cachedHandler);
            RequestPendingZones();
        }
        else
        {
            Debug.Log("⏳ Cliente MQTT no conectado. Se registrará OnConnected...");
            MQTTClient.Instance.RegisterHandler(pendingZonesTopic, cachedHandler);
            MQTTClient.Instance.OnConnected += HandleMQTTConnected;
        }
    }

    private void OnDisable()
    {
        Debug.Log("🔴 SearchZoneReceiverManager deshabilitado.");

        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(pendingZonesTopic, cachedHandler);
            MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        }
    }

    private void Update()
    {
        while (pendingJsonQueue.Count > 0)
        {
            string json;
            lock (pendingJsonQueue)
            {
                json = pendingJsonQueue.Dequeue();
                Debug.Log($"🧩 JSON dequeued para procesar: {json}");
            }
            ProcessZones(json);
        }
    }

    private void EnqueueJson(string json)
    {
        Debug.Log($"📩 JSON recibido por MQTT: {json}");

        lock (pendingJsonQueue)
        {
            pendingJsonQueue.Enqueue(json);
            Debug.Log("📥 JSON encolado para procesar.");
        }
    }

    private void ProcessZones(string json)
    {
        Debug.Log($"🔍 Intentando deserializar zonas: {json}");

        SearchZoneSummaryListWrapper wrapper;
        try
        {
            wrapper = JsonUtility.FromJson<SearchZoneSummaryListWrapper>(json);
            Debug.Log("✅ JSON deserializado correctamente.");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error en deserialización de zonas: " + ex.Message);
            return;
        }

        if (wrapper?.zones == null || wrapper.zones.Count == 0)
        {
            Debug.Log("📭 No hay zonas dentro del wrapper deserializado.");
            return;
        }

        Debug.Log($"📋 {wrapper.zones.Count} zonas recibidas. Instanciando visuales…");

        lastReceivedZones = wrapper.zones;
        UpdateZoneVisuals();
    }

    private void UpdateZoneVisuals()
    {
        Debug.Log("🔁 Ejecutando UpdateZoneVisuals…");

        foreach (var go in instantiatedZones) Destroy(go);
        instantiatedZones.Clear();

        foreach (var summary in lastReceivedZones)
        {
            var center = summary.center.ToUnityVector3();
            var size = summary.size.ToUnityVector3();

            GameObject go = Instantiate(zonePrefab, center, Quaternion.identity);
            go.transform.localScale = size;

            Debug.Log($"✅ Zona visual instanciada → ID: {summary.id}, Pos: {center}, Tamaño: {size}");
            instantiatedZones.Add(go);

            // 🔵 Generar ruta para esta zona
            if (routeGenerator != null)
            {
                routeGenerator.GenerateRouteForZone(go);
                Debug.Log($"📍 Ruta generada para la zona: {summary.id}");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró SearchRouteGenerator en escena del piloto.");
            }
        }
        // Al terminar de generar todas las rutas
        PilotRouteProgressManager progressManager = FindObjectOfType<PilotRouteProgressManager>();
        if (progressManager != null)
        {
            progressManager.RefreshRoutePoints();
            Debug.Log("🔁 PilotRouteProgressManager refrescado tras generar rutas.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró PilotRouteProgressManager para refrescar puntos.");
        }
    }


    private void RequestPendingZones()
    {
        Debug.Log("📤 [Pilot] Enviando petición de zonas pendientes…");

        var publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
        publisher.PublishMessage(pendingRequestTopic, "request_pending_zones");
    }

    private void HandleMQTTConnected()
    {
        Debug.Log("🔌 Cliente MQTT reconectado. Lanzando petición de zonas pendientes...");
        MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        RequestPendingZones();
    }
}
