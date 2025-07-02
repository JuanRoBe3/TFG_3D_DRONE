using UnityEngine;
using MQTTnet;
using Newtonsoft.Json;
using System.Collections;

public class DroneCameraPublisher : MonoBehaviour
{
    [Header("Identificador único del dron / piloto")]
    [SerializeField] private string droneId = "Pilot_01";   // ⚠️ pon aquí tu ID único

    private MQTTPublisher publisher;
    private Transform cameraTransform;

    private const float publishRate = 0.1f;                 // 10 Hz

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

        // 🔸 Incluimos el ID en el payload
        var msg = new DroneCameraTransform
        {
            id = droneId,
            pos = new SerializableVector3(cameraTransform.position),
            rot = new SerializableQuaternion(cameraTransform.rotation)
        };

        string json = JsonConvert.SerializeObject(msg);
        publisher.PublishMessage(MQTTConstants.DroneCameraTopic, json);
    }

    /// <summary>El Manager de escena llama a esto para decir qué cámara publicar.</summary>
    public void SetCamera(Transform cam) => cameraTransform = cam;
}
