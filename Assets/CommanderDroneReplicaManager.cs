using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Newtonsoft.Json;

/// <summary>
/// Gestiona las réplicas visuales de los drones activos en el comandante.
/// Instancia prefabs en cuanto llegan mensajes por MQTT (DroneCameraTransform).
/// </summary>
public class CommanderDroneReplicaManager : MonoBehaviour
{
    public static CommanderDroneReplicaManager Instance { get; private set; }

    [Header("Prefab del dron réplica")]
    [SerializeField] private GameObject commanderDroneReplicaPrefab;

    [Header("UI donde se verá la cámara (opcional)")]
    [SerializeField] private RawImage droneViewRawImage;

    private readonly Dictionary<string, CommanderDroneReplica> replicas = new();
    private readonly ConcurrentQueue<string> payloadQueue = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null");
            return;
        }

        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, OnDroneCameraMessageReceived);
        Debug.Log("📡 Suscrito a DroneCameraTopic");
    }

    private void OnDisable()
    {
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic, OnDroneCameraMessageReceived);
    }

    private void Update()
    {
        while (payloadQueue.TryDequeue(out var payload))
        {
            ProcessPayload(payload);
        }
    }

    private void OnDroneCameraMessageReceived(string payload)
    {
        payloadQueue.Enqueue(payload);
    }

    private void ProcessPayload(string payload)
    {
        Debug.Log($"📨 [Comandante] Payload recibido: {payload}");

        DroneCameraTransform data;
        try
        {
            data = JsonConvert.DeserializeObject<DroneCameraTransform>(payload);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Deserialización fallida: {ex.Message}");
            return;
        }

        if (data == null || string.IsNullOrEmpty(data.id)) return;
        if (replicas.ContainsKey(data.id)) return;

        // 🧠 Buscar el DroneData desde el registro
        DroneData droneData = DroneRegistry.Get(data.id);
        if (droneData == null)
        {
            Debug.LogError($"❌ No se encontró DroneData para el ID: {data.id}");
            return;
        }

        // 🧱 Instanciar réplica
        GameObject obj = Instantiate(commanderDroneReplicaPrefab, Vector3.zero, Quaternion.identity);
        var replica = obj.GetComponent<CommanderDroneReplica>();
        if (replica == null)
        {
            Debug.LogError("❌ Prefab no tiene componente CommanderDroneReplica.");
            return;
        }

        // 🎯 Inicializar con datos
        replica.Init(droneData);

        // 🧩 Guardar en el diccionario
        replicas[data.id] = replica;

        Debug.Log($"✅ Réplica del dron «{data.id}» creada correctamente");
    }

    /// <summary>
    /// Devuelve la réplica de un dron activo por ID.
    /// </summary>
    public CommanderDroneReplica GetReplicaByDroneId(string droneId)
    {
        replicas.TryGetValue(droneId, out var replica);
        return replica;
    }
}
