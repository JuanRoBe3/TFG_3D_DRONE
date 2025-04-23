using UnityEngine;
using TMPro;
using System.Collections.Generic;

//ELIMINAR PORQUE YA NO SIRVE DE NADA EN TEORÍA

public class PendingTasksUI : MonoBehaviour
{
    public TextMeshProUGUI tasksText;

    void OnEnable()
    {
        // Suscribirse al topic de tareas pendientes
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksTopic, OnTasksReceived);

        // Publicar petición de tareas pendientes
        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingTasksRequestTopic, "request_pending_tasks");
    }

    void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksTopic);
    }

    void OnTasksReceived(string json)
    {
        TaskSummaryListWrapper wrapper = JsonUtility.FromJson<TaskSummaryListWrapper>(json);
        Display(wrapper.tasks);
    }

    void Display(List<TaskSummary> tasks)
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
