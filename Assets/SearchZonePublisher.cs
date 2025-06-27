using UnityEngine;

public class SearchZonePublisher : MonoBehaviour
{
    private MQTTPublisher publisher;

    void Start()
    {
        publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
    }

    public void PublishZone(Vector3 center, Vector3 size)
    {
        var payload = JsonUtility.ToJson(new ZonePayload(center, size));
        publisher.PublishMessage(MQTTConstants.Zone, payload);
    }

    [System.Serializable]
    private struct ZonePayload
    {
        public float x, y, z, sx, sy, sz;
        public ZonePayload(Vector3 c, Vector3 s)
        {
            x = c.x; y = c.y; z = c.z;
            sx = s.x; sy = s.y; sz = s.z;
        }
    }
}
