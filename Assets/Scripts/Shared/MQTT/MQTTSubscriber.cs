using System;
using UnityEngine;

public class MQTTSubscriber : MonoBehaviour
{
    private void Start()
    {
        MQTTClient.Instance.OnMessageReceived += HandleMessageReceived;
    }

    private void HandleMessageReceived(string topic, string payload)
    {
        Debug.Log($"📨 MQTTSubscriber recibió: {topic}");

        switch (topic)
        {
            case MQTTConstants.SelectedTaskTopic:
                HandleSelectedTask(payload);
                break;

            default:
                Debug.LogWarning($"⚠️ Tópico sin handler: {topic}");
                break;
        }
    }

    private void HandleSelectedTask(string payload)
    {
        var msg = JsonUtility.FromJson<TaskStatusUpdateMessage>(payload);

        var task = TaskRegistry.GetTaskById(msg.taskId);
        if (task == null)
        {
            Debug.LogWarning($"❌ Tarea no encontrada en TaskRegistry: {msg.taskId}");
            return;
        }

        task.status = msg.newStatus;
        task.assignedDrone = DroneRegistry.GetDroneById(msg.droneId);

        Debug.Log($"✅ Estado actualizado en TaskRegistry: {task.title} → {task.status}");

        // ❌ No tocamos UI ni usamos .Instance
        // TaskListManager.Instance?.RefreshTaskList();
    }

    private void OnDestroy()
    {
        if (MQTTClient.Instance != null)
            MQTTClient.Instance.OnMessageReceived -= HandleMessageReceived;
    }
}
