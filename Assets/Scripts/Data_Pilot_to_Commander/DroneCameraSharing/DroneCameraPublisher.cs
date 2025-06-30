using UnityEngine;
using MQTTnet;
using Newtonsoft.Json;
using System.Collections;

public class DroneCameraPublisher : MonoBehaviour
{
    private MQTTPublisher publisher;
    private Transform firstPersonCameraTransform;
    private Transform topDownCameraTransform;

    const float publishRate = 0.1f;

    void Start()
    {
        if (!RoleSelection.IsPilot) { enabled = false; return; }    // ⬅️ filtro

        var client = MQTTClient.Instance.GetClient();
        if (client == null) { Debug.LogError("❌ MQTT client null"); return; }

        publisher = new MQTTPublisher(client);
        StartCoroutine(PublishLoop());
    }

    IEnumerator PublishLoop()
    {
        var wait = new WaitForSeconds(publishRate);
        while (true) { PublishCameraData(); yield return wait; }
    }

    void PublishCameraData()
    {
        if (!firstPersonCameraTransform || !topDownCameraTransform) return;

        var msg = new CameraDataMessage
        {
            firstPersonPos = new SerializableVector3(firstPersonCameraTransform.position),
            firstPersonRot = new SerializableQuaternion(firstPersonCameraTransform.rotation),
            topDownPos = new SerializableVector3(topDownCameraTransform.position),
            topDownRot = new SerializableQuaternion(topDownCameraTransform.rotation)
        };

        publisher.PublishMessage(MQTTConstants.DroneCameraTopic,
                                 JsonConvert.SerializeObject(msg));
    }

    public void SetCameras(Transform fp, Transform td)
    {
        firstPersonCameraTransform = fp;
        topDownCameraTransform = td;
    }
}
