using UnityEngine;

public class SearchZoneReceiverProxy : MonoBehaviour
{
    public static SearchZoneReceiverProxy Instance { get; private set; }

    private bool hasProcessedPendingZones = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        MQTTClient.EnsureExists();

        RegisterHandler();
        MQTTClient.Instance.OnReconnectCompleted += RegisterHandler;

        Debug.Log("✅ [ReceiverProxy] Listo con reconexión automática");
    }

    void Update()
    {
        if (!hasProcessedPendingZones && PendingZoneBuffer.Instance != null && PendingZoneBuffer.Instance.HasPendingZones())
        {
            var manager = FindObjectOfType<SearchZoneReplicaManager>();
            if (manager != null)
            {
                Debug.Log("🕒 Manager encontrado en Update → procesando zonas pendientes");
                PendingZoneBuffer.Instance.ConsumeAll(OnZoneReceived);
                hasProcessedPendingZones = true;
            }
        }
    }

    void OnDestroy()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
            MQTTClient.Instance.OnReconnectCompleted -= RegisterHandler;
        }

        if (Instance == this) Instance = null;
    }

    private void RegisterHandler()
    {
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
        Debug.Log("🔁 [ReceiverProxy] Handler registrado (o re-registrado) tras conexión/reconexión");
    }

    private void OnZoneReceived(string json)
    {
        var data = JsonUtility.FromJson<SearchZoneData>(json);
        if (data == null)
        {
            Debug.LogError("❌ JSON malformado");
            return;
        }

        var manager = FindObjectOfType<SearchZoneReplicaManager>();
        if (manager != null)
        {
            Debug.Log("📨 Manager encontrado → reenviando mensaje");
            manager.SendMessage("OnZoneReceived", json);
        }
        else
        {
            Debug.Log("📥 Manager NO presente → zona almacenada en buffer");
            PendingZoneBuffer.Instance?.Add(data);
        }
    }
}
