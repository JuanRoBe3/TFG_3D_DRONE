using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class TaskListManager : MonoBehaviour
{
    [Header("Prefabs y referencias")]
    public GameObject taskItemPrefab;
    public Transform contentParent;
    public RectTransform scrollViewRectTransform;
    public TaskEditorUI taskEditorUI;

    [Header("Altura relativa de las tareas")]
    [Range(0.1f, 1f)]
    public float taskHeightPercentage = 0.3f;

    private void OnEnable()
    {
        // 📥 El comandante escucha peticiones de tareas del piloto
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksRequestTopic, OnPendingTasksRequestReceived);
    }

    private void OnDisable()
    {
        // 🔇 Se desuscribe al salir de escena
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksRequestTopic);
    }

    private void OnPendingTasksRequestReceived(string _)
    {
        Debug.Log("📨 Petición de tareas recibida por MQTT desde el piloto.");
        PublishPendingTasks(); // Reutilizamos tu método
    }

    private void Start()
    {
        AdjustAllTaskItems();

        foreach (TaskItemUI taskUI in contentParent.GetComponentsInChildren<TaskItemUI>())
        {
            var data = taskUI.TaskData;

            if (data != null)
            {
                if (string.IsNullOrEmpty(data.id))
                {
                    data.id = Guid.NewGuid().ToString();
                    Debug.Log($"🆕 ID generado para tarea: {data.title} => {data.id}");
                }

                taskUI.Setup(data, this);
            }
            else
            {
                Debug.LogWarning("⚠️ Tarea sin datos. No será incluida.");
            }
        }

        PublishPendingTasks(); // Primera publicación al cargar
    }

    private void OnRectTransformDimensionsChange()
    {
        AdjustAllTaskItems();
    }

    public void OpenCreateTask()
    {
        var drones = CommanderDroneManager.Instance.GetAvailableDrones();
        taskEditorUI.SetAvailableDrones(drones);

        taskEditorUI.Show((taskData) =>
        {
            GameObject item = Instantiate(taskItemPrefab, contentParent);
            AdjustTaskItemHeight(item);
            TaskItemUI taskUI = item.GetComponent<TaskItemUI>();
            if (taskUI != null)
                taskUI.Setup(taskData, this);

            PublishPendingTasks();
        });
    }

    public void EditTask(TaskData existingData, TaskItemUI itemUI)
    {
        var drones = CommanderDroneManager.Instance.GetAvailableDrones();
        taskEditorUI.SetAvailableDrones(drones);

        taskEditorUI.Show((updatedData) =>
        {
            existingData.title = updatedData.title;
            existingData.description = updatedData.description;
            existingData.status = updatedData.status;
            existingData.assignedDrone = updatedData.assignedDrone;

            itemUI.Setup(existingData, this);
            PublishPendingTasks();
        }, existingData);
    }

    private void AdjustTaskItemHeight(GameObject taskItem)
    {
        float containerHeight = scrollViewRectTransform.rect.height;
        float itemHeight = containerHeight * taskHeightPercentage;

        LayoutElement layout = taskItem.GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.preferredHeight = itemHeight;
            layout.flexibleHeight = 0;
        }
    }

    public void AdjustAllTaskItems()
    {
        float containerHeight = scrollViewRectTransform.rect.height;
        float itemHeight = containerHeight * taskHeightPercentage;

        foreach (Transform child in contentParent)
        {
            LayoutElement layout = child.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.preferredHeight = itemHeight;
                layout.flexibleHeight = 0;
            }
        }
    }

    public List<TaskData> GetAllTasks()
    {
        List<TaskData> all = new();
        foreach (Transform child in contentParent)
        {
            TaskItemUI taskUI = child.GetComponent<TaskItemUI>();
            if (taskUI != null && taskUI.TaskData != null)
                all.Add(taskUI.TaskData);
        }
        return all;
    }

    public void PublishPendingTasks()
    {
        List<TaskData> all = GetAllTasks();
        foreach (var task in all)
        {
            Debug.Log($"🔍 Tarea detectada: {task.title}, estado: '{task.status}'");
        }

        List<TaskData> pending = all.FindAll(t => t.status == "To be executed");
        List<TaskSummary> summaries = pending.ConvertAll(t => new TaskSummary(t));
        var wrapper = new TaskSummaryListWrapper { tasks = summaries };

        string json = JsonUtility.ToJson(wrapper);

        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingTasksTopic, json);

        Debug.Log($"📤 Publicadas {pending.Count} tareas pendientes");
    }
}
