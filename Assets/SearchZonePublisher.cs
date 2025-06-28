using UnityEngine;

public class SearchZonePublisher : MonoBehaviour
{
    private MQTTPublisher publisher;

    void Start()
    {
        publisher = GetComponent<MQTTPublisher>();
    }

    public void PublishZone(Vector3 center, Vector3 size)
    {
        var payload = new ZonePayload
        {
            center = new Vector3Payload { x = center.x, y = center.y, z = center.z },
            size = new Vector3Payload { x = size.x, y = size.y, z = size.z }
        };

        string json = JsonUtility.ToJson(payload);
        publisher.PublishMessage(MQTTConstants.SearchingZone, json);

        Debug.Log($"📤 Zona publicada en {MQTTConstants.SearchingZone}");
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
