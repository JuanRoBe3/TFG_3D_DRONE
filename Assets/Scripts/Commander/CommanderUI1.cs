using UnityEngine;
using Newtonsoft.Json;

public class CommanderUI1 : MonoBehaviour
{
    void OnEnable()
    {
        if (MQTTClient.Instance != null)
        {
            // ✅ Registramos función nombrada como handler
            MQTTClient.Instance.RegisterHandler(MQTTConstants.SelectedTaskTopic, OnSelectedTaskReceived);
        }
    }

    void OnDisable()
    {
        if (MQTTClient.Instance != null)
        {
            // ✅ Desregistramos la misma función nombrada
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.SelectedTaskTopic, OnSelectedTaskReceived);
        }
    }

    // ✅ Esta es la función (handler) que se llama cuando llega el mensaje MQTT
    private void OnSelectedTaskReceived(string payload)
    {
        Debug.Log($"📩 [CommanderUI1] Mensaje recibido en {MQTTConstants.SelectedTaskTopic}: {payload}");

        // Ejemplo: parsear el payload y actualizar algo
        try
        {
            var msg = JsonConvert.DeserializeObject<TaskStatusUpdateMessage>(payload);
            if (msg == null) return;

            // Lógica de actualización, por ejemplo:
            var task = TaskRegistry.GetTaskById(msg.taskId);
            if (task != null)
            {
                task.status = msg.newStatus;
                task.assignedDrone = DroneRegistry.GetDroneById(msg.droneId);

                Debug.Log($"✅ Estado actualizado: {task.title} → {task.status}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error al procesar mensaje: {ex.Message}");
        }
    }
}
