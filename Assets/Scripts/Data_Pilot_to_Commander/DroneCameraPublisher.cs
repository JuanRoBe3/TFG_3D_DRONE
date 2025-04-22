using UnityEngine;
using MQTTnet;
using Newtonsoft.Json;

public class DroneCameraPublisher : MonoBehaviour
{
    private MQTTPublisher publisher;
    private Transform firstPersonCameraTransform;
    private Transform topDownCameraTransform;

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
        if (firstPersonCameraTransform == null || topDownCameraTransform == null)
        {
            Debug.LogWarning("⚠️ Cámaras no asignadas todavía.");
            return;
        }

        var msg = new CameraDataMessage
        {
            firstPersonPos = new SerializableVector3(firstPersonCameraTransform.position),
            firstPersonRot = new SerializableQuaternion(firstPersonCameraTransform.rotation),
            topDownPos = new SerializableVector3(topDownCameraTransform.position),
            topDownRot = new SerializableQuaternion(topDownCameraTransform.rotation)
        };


        string json = JsonConvert.SerializeObject(msg);
        Debug.Log($"📤 Enviando cámaras: {json}");
        publisher.PublishMessage(MQTTConstants.DroneCameraTopic, json);
    }

    public void SetCameras(Transform fpCam, Transform tdCam)
    {
        firstPersonCameraTransform = fpCam;
        topDownCameraTransform = tdCam;
        Debug.Log("📸 Cámaras asignadas al DroneCameraPublisher.");
    }
}
