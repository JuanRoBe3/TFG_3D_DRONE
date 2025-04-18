using UnityEngine;
using MQTTnet;

/// <summary>
/// Publica periódicamente la posición y rotación de las cámaras del piloto por MQTT.
/// </summary>
public class DroneCameraPublisher : MonoBehaviour
{
    public Transform firstPersonCameraTransform;
    public Transform topDownCameraTransform;

    private MQTTPublisher publisher;

    private void Start()
    {
        var client = MQTTClient.Instance.GetClient();

        if (client == null)
        {
            Debug.LogError("❌ No MQTT client found.");
            return;
        }

        publisher = new MQTTPublisher(client);
        InvokeRepeating(nameof(PublishCameraData), 0f, 0.1f);
    }

    private void PublishCameraData()
    {
        Vector3 pos1 = firstPersonCameraTransform.position;
        Quaternion rot1 = firstPersonCameraTransform.rotation;
        Vector3 pos2 = topDownCameraTransform.position;
        Quaternion rot2 = topDownCameraTransform.rotation;

        var message = new CameraDataMessage(pos1, rot1, pos2, rot2);
        string json = JsonUtility.ToJson(message);

        publisher.PublishMessage(MQTTConstants.DroneCameraTopic, json);
    }
}
