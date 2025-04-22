using UnityEngine;
using Newtonsoft.Json;

public class DroneCameraReplicator : MonoBehaviour
{
    [Header("Referencias a las cámaras del comandante")]
    public Transform commanderFirstPersonCamera;
    public Transform commanderTopDownCamera;

    // ✅ Almacenamos el último mensaje recibido
    private CameraDataMessage pendingData = null;
    private bool hasNewData = false;

    private void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null en DroneCameraReplicator.OnEnable");
            return;
        }

        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, HandleCameraPayload);
        Debug.Log("📡 DroneCameraReplicator ACTIVADO");
    }

    private void OnDisable()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic);
            Debug.Log("📴 DroneCameraReplicator DESACTIVADO y handler limpiado");
        }
    }

    // ⚙️ Maneja el mensaje desde el hilo MQTT, pero solo almacena los datos
    private void HandleCameraPayload(string payload)
    {
        if (string.IsNullOrEmpty(payload))
        {
            Debug.LogWarning("⚠️ Payload vacío en DroneCameraReplicator.");
            return;
        }

        try
        {
            Debug.Log("📨 Payload recibido para deserializar:\n" + payload);
            pendingData = JsonConvert.DeserializeObject<CameraDataMessage>(payload);
            hasNewData = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error al deserializar con Newtonsoft: " + ex.Message);
        }
    }

    // ✅ Aplicamos transformaciones en el hilo principal
    private void Update()
    {
        if (!hasNewData || pendingData == null) return;

        if (commanderFirstPersonCamera == null || commanderTopDownCamera == null)
        {
            Debug.LogError("❌ Cámaras del comandante no asignadas.");
            hasNewData = false;
            return;
        }

        // Aplicar transformaciones
        commanderFirstPersonCamera.position = pendingData.firstPersonPos.ToUnityVector3();
        commanderFirstPersonCamera.rotation = pendingData.firstPersonRot.ToUnityQuaternion();
        commanderTopDownCamera.position = pendingData.topDownPos.ToUnityVector3();
        commanderTopDownCamera.rotation = pendingData.topDownRot.ToUnityQuaternion();

        // Activar cámaras si estaban desactivadas
        Camera fpCam = commanderFirstPersonCamera.GetComponent<Camera>();
        Camera tdCam = commanderTopDownCamera.GetComponent<Camera>();

        if (fpCam != null && !fpCam.enabled)
        {
            fpCam.enabled = true;
            Debug.Log("🎥 Cámara FP activada");
        }

        if (tdCam != null && !tdCam.enabled)
        {
            tdCam.enabled = true;
            Debug.Log("🎥 Cámara TD activada");
        }

        Debug.Log("📥 Info de cámara aplicada con éxito");
        Debug.Log("📍 FP Pos: " + pendingData.firstPersonPos.ToUnityVector3() + " | Rot: " + pendingData.firstPersonRot.ToUnityQuaternion().eulerAngles);
        Debug.Log("📍 TD Pos: " + pendingData.topDownPos.ToUnityVector3() + " | Rot: " + pendingData.topDownRot.ToUnityQuaternion().eulerAngles);

        hasNewData = false;
    }
}
