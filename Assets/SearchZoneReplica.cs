using UnityEngine;

public class SearchZoneReplica : MonoBehaviour
{
    [Tooltip("Prefab de zona de búsqueda")]
    [SerializeField] private GameObject zonePrefab;

    void OnEnable()
    {
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
    }

    void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.SearchingZone);
    }

    void OnZoneReceived(string payload)
    {
        if (!TryParse(payload, out Vector3 center, out Vector3 size))
        {
            Debug.LogWarning("❌ No se pudo parsear zona: " + payload);
            return;
        }

        GameObject zone = Instantiate(zonePrefab, center, Quaternion.identity);
        zone.transform.localScale = size;

        Debug.Log($"📦 Zona recibida e instanciada: {center} | {size}");
    }

    bool TryParse(string json, out Vector3 center, out Vector3 size)
    {
        center = size = Vector3.zero;
        try
        {
            var data = JsonUtility.FromJson<ZonePayload>(json);
            center = new Vector3(data.x, data.y, data.z);
            size = new Vector3(data.sx, data.sy, data.sz);
            return true;
        }
        catch { return false; }
    }

    [System.Serializable]
    private struct ZonePayload
    {
        public float x, y, z, sx, sy, sz;
    } 
}
