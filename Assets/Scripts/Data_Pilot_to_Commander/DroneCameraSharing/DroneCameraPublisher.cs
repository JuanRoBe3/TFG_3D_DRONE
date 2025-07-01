using UnityEngine;
using MQTTnet;
using Newtonsoft.Json;
using System.Collections;

public class DroneCameraPublisher : MonoBehaviour
{
    private MQTTPublisher publisher;
    private Transform cameraTransform;

    const float publishRate = 0.1f;

    void Start()
    {
        if (!RoleSelection.IsPilot)
        {
            enabled = false;
            return;
        }

        var client = MQTTClient.Instance.GetClient();
        if (client == null)
        {
            Debug.LogError("❌ MQTT client null");
            return;
        }

        publisher = new MQTTPublisher(client);
        StartCoroutine(PublishLoop());
    }

    IEnumerator PublishLoop()
    {
        var wait = new WaitForSeconds(publishRate);
        while (true)
        {
            PublishCameraData();
            yield return wait;
        }
    }

    void PublishCameraData()
    {
        if (cameraTransform == null) return;

        var msg = new DroneCameraTransform
        {
            pos = new SerializableVector3(cameraTransform.position),
            rot = new SerializableQuaternion(cameraTransform.rotation)
        };

        string json = JsonConvert.SerializeObject(msg);
        publisher.PublishMessage(MQTTConstants.DroneCameraTopic, json);
    }

    public void SetCamera(Transform cam)
    {
        cameraTransform = cam;
    }
}
