using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Newtonsoft.Json;

public class CommanderDroneReplicaManager : MonoBehaviour
{
    [Header("Prefab del dron réplica")]
    [SerializeField] private GameObject commanderDroneReplicaPrefab;

    [Header("UI donde se verá la cámara (opcional)")]
    [SerializeField] private RawImage droneViewRawImage;

    private readonly Dictionary<string, CommanderDroneReplica> replicas = new();
    private readonly ConcurrentQueue<string> payloadQueue = new();

    // ✅ CAMBIO: hemos dado nombre al handler MQTT
    private void OnDroneCameraMessageReceived(string payload)
    {
        payloadQueue.Enqueue(payload);
    }

    void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null");
            return;
        }

        // ✅ CAMBIO: usamos función nombrada en lugar de lambda anónima
        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, OnDroneCameraMessageReceived);
        Debug.Log("📡 Suscrito a DroneCameraTopic");
    }

    void OnDisable()
    {
        // ✅ CAMBIO: usamos la misma función nombrada para desregistrar correctamente
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic, OnDroneCameraMessageReceived);
    }

    void Update()
    {
        while (payloadQueue.TryDequeue(out var payload))
        {
            ProcessPayload(payload);
        }
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

        // 🧠 Buscar el DroneData desde el registro central
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
            Debug.LogError("❌ Prefab no tiene CommanderDroneReplica.");
            return;
        }

        // 🎯 Inicializar con datos
        replica.Init(droneData);

        // 🧩 Guardar instancia
        replicas[data.id] = replica;

        Debug.Log($"✅ Réplica del dron «{data.id}» creada correctamente");
    }
}
