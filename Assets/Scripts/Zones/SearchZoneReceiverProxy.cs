using UnityEngine;

public class SearchZoneReceiverProxy : MonoBehaviour
{
    public static SearchZoneReceiverProxy Instance { get; private set; }

    void Awake()
    {
        //— Singleton persistente —
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //— Suscripción MQTT —
        MQTTClient.EnsureExists();
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
        Debug.Log("✅ [ReceiverProxy] Suscrito a MQTTConstants.SearchingZone");
    }

    void OnDestroy()
    {
        //— Evita fugas de delegados —
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);

        if (Instance == this) Instance = null;
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
