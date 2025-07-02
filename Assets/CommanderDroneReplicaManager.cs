using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;   // 👈
using Newtonsoft.Json;

public class CommanderDroneReplicaManager : MonoBehaviour
{
    [Header("Prefab del dron réplica")]
    [SerializeField] private GameObject commanderDroneReplicaPrefab;

    [Header("UI donde se verá la cámara")]
    [SerializeField] private RawImage droneViewRawImage;

    private readonly Dictionary<string, CommanderDroneReplica> replicas = new();
    private readonly ConcurrentQueue<string> payloadQueue = new();      // 👈 cola segura

    void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null");
            return;
        }

        // Suscripción sigue igual
        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic,
                                            payload => payloadQueue.Enqueue(payload));
        Debug.Log("📡 Suscrito a DroneCameraTopic");
    }

    void OnDisable()
    {
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic);
    }

    // Procesamos la cola EN EL HILO PRINCIPAL
    void Update()
    {
        while (payloadQueue.TryDequeue(out var payload))
        {
            ProcessPayload(payload);
        }
    }

    // ---------- Lógica original, sin cambios salvo que ahora se llama desde Update ----------
    private void ProcessPayload(string payload)
    {
        // Logs de depuración
        Debug.Log($"📨 [Comandante] Payload recibido (main thread): {payload}");

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

        // Instanciar réplica en hilo principal
        var obj = Instantiate(commanderDroneReplicaPrefab, Vector3.zero, Quaternion.identity);
        var replica = obj.GetComponent<CommanderDroneReplica>();

        if (replica == null)
        {
            Debug.LogError("❌ Prefab sin CommanderDroneReplica.");
            return;
        }

        replica.Init(data.id);

        // RenderTexture y RawImage
        var rt = RTFactory.New();
        replica.GetCamera().targetTexture = rt;
        droneViewRawImage.texture = rt;

        replicas[data.id] = replica;
        Debug.Log($"✅ Réplica FINALIZADA para dron «{data.id}»");
    }
}
