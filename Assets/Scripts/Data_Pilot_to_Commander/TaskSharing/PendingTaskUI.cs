using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Muestra una lista de tareas pendientes cuando llegan por MQTT.
/// </summary>
public class PendingTasksUI : MonoBehaviour
{
    public TextMeshProUGUI tasksText;

    void OnEnable()
    {
        // ✅ Registramos el handler nombrado
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksTopic, OnTasksReceived);

        // ✅ Publicamos petición de tareas pendientes
        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingTasksRequestTopic, "request_pending_tasks");
    }

    void OnDisable()
    {
        // ✅ Desregistramos el handler correctamente
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksTopic, OnTasksReceived);
    }

    // ✅ Handler nombrado para procesar tareas recibidas
    private void OnTasksReceived(string json)
    {
        TaskSummaryListWrapper wrapper = JsonUtility.FromJson<TaskSummaryListWrapper>(json);
        Display(wrapper.tasks);
    }

    private void Display(List<TaskSummary> tasks)
    {
        if (tasks == null || tasks.Count == 0)
        {
            tasksText.text = "No pending tasks";
            return;
        }

        string result = "";
        foreach (var task in tasks)
        {
            result += $"- {task.title} ({task.drone})\n";
        }

        tasksText.text = result;
    }
}
