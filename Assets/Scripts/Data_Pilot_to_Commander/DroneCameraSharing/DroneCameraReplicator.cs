using UnityEngine;
using Newtonsoft.Json;

public class DroneCameraReplicator : MonoBehaviour
{
    [Header("Transform que se moverá en el comandante")]
    public Transform targetToMove;

    private string droneId;                     // 🔸 NUEVO
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
            if (data == null || data.id != droneId) return;   // 🔸 filtrado

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
        if (!hasNewData || pendingData == null || targetToMove == null) return;

        targetToMove.position = pendingData.pos.ToUnityVector3();
        targetToMove.rotation = pendingData.rot.ToUnityQuaternion();
        hasNewData = false;
    }

    // llamados desde CommanderDroneReplica.Init(...)
    public void SetCamera(Transform cam) => targetToMove = cam;
    public void SetDroneId(string id) => droneId = id;   // 🔸
}
