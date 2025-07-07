using UnityEngine;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Collections;

public class DroneCameraPublisher : MonoBehaviour
{
    [Header("Identificador único del dron / piloto")]
    [SerializeField] private string droneId = "";          // ← vacío por defecto

    private MQTTPublisher publisher;
    private Transform cameraTransform;
    private bool isReady = false;

    private const float publishRate = 0.1f;                // 10 Hz

    /*  NO hacemos nada en Start; se arrancará con Initialize()  */
    void Start()
    {
        if (!RoleSelection.IsPilot) { enabled = false; return; }
    }

    public void Initialize(IMqttClient client, string id, Transform cam)
    {
        if (client == null || cam == null || string.IsNullOrEmpty(id))
        {
            Debug.LogError("❌ DroneCameraPublisher.Initialize: parámetros inválidos");
            return;
        }

        droneId = id;
        cameraTransform = cam;
        publisher = new MQTTPublisher(client);

        if (!isReady) StartCoroutine(PublishLoop());
        isReady = true;

        Debug.Log($"📡 DroneCameraPublisher listo. ID = {droneId}");
    }

    public void SetCamera(Transform cam)
    {
        cameraTransform = cam;
        // Si el publisher ya estaba listo, no necesitas nada más.
        // Si todavía no se había inicializado, 'Initialize' lo hará.
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
        if (!isReady) return;

        var msg = new DroneCameraTransform
        {
            id = droneId,
            pos = new SerializableVector3(cameraTransform.position),
            rot = new SerializableQuaternion(cameraTransform.rotation)
        };

        string json = JsonConvert.SerializeObject(msg);
        publisher.PublishMessage(MQTTConstants.DroneCameraTopic, json);
    }
}
