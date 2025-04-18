using UnityEngine;

/// <summary>
/// Recibe por MQTT la informaci�n de posici�n y rotaci�n de las c�maras del piloto y la aplica a las del comandante.
/// </summary>
public class DroneCameraReplicator : MonoBehaviour
{
    public Transform commanderFirstPersonCamera;
    public Transform commanderTopDownCamera;

    private void OnEnable()
    {
        MQTTClient.Instance.OnMessageReceived += HandleMessageReceived;
    }

    private void OnDisable()
    {
        MQTTClient.Instance.OnMessageReceived -= HandleMessageReceived;
    }

    private void HandleMessageReceived(string topic, string payload)
    {
        if (topic != MQTTConstants.DroneCameraTopic)
            return;

        var data = JsonUtility.FromJson<CameraDataMessage>(payload);
        commanderFirstPersonCamera.position = data.firstPersonPos;
        commanderFirstPersonCamera.rotation = data.firstPersonRot;
        commanderTopDownCamera.position = data.topDownPos;
        commanderTopDownCamera.rotation = data.topDownRot;
    }
}
