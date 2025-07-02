using UnityEngine;
using Newtonsoft.Json;

public class DroneCameraReplicator : MonoBehaviour
{
    [Header("Referencias de movimiento y rotación")]
    public Transform rootToMove;       // Mueve todo el dron (posición)
    public Transform visualToRotate;   // Solo gira en Y (esfera)
    public Transform fpvCam;           // Aplica rotación completa

    private string droneId;
    private DroneCameraTransform pendingData;
    private bool hasNewData = false;

    void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null");
            return;
        }

        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, HandlePayload);
    }

    void OnDisable()
    {
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic);
    }

    private void HandlePayload(string payload)
    {
        if (string.IsNullOrEmpty(payload)) return;

        try
        {
            var data = JsonConvert.DeserializeObject<DroneCameraTransform>(payload);
            if (data == null || data.id != droneId) return;

            pendingData = data;
            hasNewData = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error al deserializar: " + ex.Message);
        }
    }

    void Update()
    {
        if (!hasNewData || pendingData == null) return;

        Vector3 pos = pendingData.pos.ToUnityVector3();
        Quaternion fullRot = pendingData.rot.ToUnityQuaternion();

        // 1. Posición real
        if (rootToMove != null)
            rootToMove.position = pos;

        // 2. Solo yaw (rotación en Y) para el visual
        if (visualToRotate != null)
        {
            Vector3 euler = fullRot.eulerAngles;
            visualToRotate.rotation = Quaternion.Euler(0, euler.y, 0);
        }

        // 3. Rotación completa para la cámara
        if (fpvCam != null)
            fpvCam.rotation = fullRot;

        hasNewData = false;
    }

    // Llamados desde CommanderDroneReplica
    public void SetDroneId(string id) => droneId = id;
    public void SetRoot(Transform t) => rootToMove = t;
    public void SetVisual(Transform t) => visualToRotate = t;
    public void SetFPVCamera(Transform t) => fpvCam = t;
}
