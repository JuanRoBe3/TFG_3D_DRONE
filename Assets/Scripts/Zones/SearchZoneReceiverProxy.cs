using UnityEngine;

public class SearchZoneReceiverProxy : MonoBehaviour
{
    void Awake()
    {
        MQTTClient.EnsureExists();
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
        Debug.Log("🛰️ [Proxy] Suscrito a zonas de búsqueda");
    }

    void OnDestroy()
    {
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
    }

    private void OnZoneReceived(string json)
    {
        var manager = FindObjectOfType<SearchZoneReplicaManager>();
        if (manager != null)
        {
            manager.OnZoneReceived(json); // si está público
        }
        else
        {
            Debug.LogWarning("⚠️ No hay manager visual activo para instanciar zonas");
        }
    }
}
