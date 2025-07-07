using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Recibe los datos de la cámara del dron por MQTT y los aplica a la réplica visual.
/// </summary>
public class DroneCameraReplicator : MonoBehaviour
{
    [Header("Transformaciones asignadas por CommanderDroneReplica")]
    [SerializeField] private Transform droneRootTransform;        // Posición completa del dron
    [SerializeField] private Transform visualYawOnlyTransform;    // Rotación solo en eje Y (visual)
    [SerializeField] private Transform fpvCameraTransform;        // Rotación completa (cámara del piloto)

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

        // ✅ Registramos handler nombrado
        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, HandlePayload);
    }

    void OnDisable()
    {
        // ✅ Corregido: usamos mismo handler nombrado al desregistrar
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic, HandlePayload);
    }

    // ✅ Handler nombrado para procesar el payload recibido
    private void HandlePayload(string payload)
    {
        if (string.IsNullOrEmpty(payload)) return;

        try
        {
            var data = JsonConvert.DeserializeObject<DroneCameraTransform>(payload);
            if (data == null) return;

            if (data.id != droneId) return; // Ignorar si no es para este dron

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
        if (!hasNewData) return;

        Vector3 position = pendingData.pos.ToUnityVector3();
        Quaternion fullRotation = pendingData.rot.ToUnityQuaternion();

        // 1. Posición general
        if (droneRootTransform != null)
            droneRootTransform.position = position;

        // 2. Rotación solo yaw para la esfera visual
        if (visualYawOnlyTransform != null)
        {
            float yaw = fullRotation.eulerAngles.y;
            visualYawOnlyTransform.rotation = Quaternion.Euler(0, yaw, 0);
        }

        // 3. Rotación completa para la cámara
        if (fpvCameraTransform != null)
            fpvCameraTransform.rotation = fullRotation;

        hasNewData = false;
    }

    // Métodos públicos para configuración
    public void SetDroneId(string id) => droneId = id;
    public void SetRoot(Transform t) => droneRootTransform = t;
    public void SetVisual(Transform t) => visualYawOnlyTransform = t;
    public void SetFPVCamera(Transform t) => fpvCameraTransform = t;
}
