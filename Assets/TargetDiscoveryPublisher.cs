using UnityEngine;
using System.Threading.Tasks;

public class TargetDiscoveryPublisher : MonoBehaviour
{
    public static TargetDiscoveryPublisher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async Task PublishDiscoveredTarget(string targetId, Vector3 camPos, Quaternion camRot)
    {
        var message = new TargetDiscoveryMessage
        {
            targetId = targetId,
            cameraPosition = new SerializableVector3(camPos),
            cameraRotation = new SerializableQuaternion(camRot)
        };

        string json = JsonUtility.ToJson(message);

        var publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
        await publisher.PublishMessage(MQTTConstants.DiscoveredTargetTopic, json);

        Debug.Log($"📡 Piloto publicó target {targetId} con cámara: {camPos} | {camRot.eulerAngles}");
    }
}
