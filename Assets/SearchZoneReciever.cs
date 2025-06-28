using UnityEngine;

public class SearchZoneReceiver : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;

    void Start()
    {
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
        Debug.Log("📡 SearchZoneReceiver escuchando " + MQTTConstants.SearchingZone);
    }

    private void OnZoneReceived(string json)
    {
        ZonePayload payload = JsonUtility.FromJson<ZonePayload>(json);

        Vector3 center = new Vector3(payload.center.x, payload.center.y, payload.center.z);
        Vector3 size = new Vector3(payload.size.x, payload.size.y, payload.size.z);

        GameObject zone = Instantiate(zonePrefab, center, Quaternion.identity);
        zone.transform.localScale = size;

        Debug.Log($"📦 Zona recibida. Centro: {center}, Escala: {size}");
    }

    [System.Serializable]
    private class ZonePayload
    {
        public Vector3Payload center;
        public Vector3Payload size;
    }

    [System.Serializable]
    private class Vector3Payload
    {
        public float x, y, z;
    }
}
