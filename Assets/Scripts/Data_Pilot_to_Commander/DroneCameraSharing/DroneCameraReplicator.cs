using UnityEngine;
using Newtonsoft.Json;

public class DroneCameraReplicator : MonoBehaviour
{
    [Header("Transform que se moverá en el comandante")]
    public Transform targetToMove; // Puede ser la cámara o una esfera

    private DroneCameraTransform pendingData;
    private bool hasNewData = false;

    private void OnEnable()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError("❌ MQTTClient.Instance es null");
            return;
        }

        MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, HandlePayload);
        Debug.Log("📡 DroneCameraReplicator ACTIVADO");
    }

    private void OnDisable()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic);
            Debug.Log("📴 DroneCameraReplicator DESACTIVADO");
        }
    }

    private void HandlePayload(string payload)
    {
        if (string.IsNullOrEmpty(payload)) return;

        try
        {
            pendingData = JsonConvert.DeserializeObject<DroneCameraTransform>(payload);
            hasNewData = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error al deserializar: " + ex.Message);
        }
    }

    private void Update()
    {
        if (!hasNewData || pendingData == null) return;

        if (targetToMove == null)
        {
            Debug.LogError("❌ targetToMove no asignado");
            hasNewData = false;
            return;
        }

        targetToMove.position = pendingData.pos.ToUnityVector3();
        targetToMove.rotation = pendingData.rot.ToUnityQuaternion();

        hasNewData = false;
    }
}
