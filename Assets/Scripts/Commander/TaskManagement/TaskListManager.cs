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

    private static TaskItemUI selectedTaskUI = null;

    private void OnEnable()
    {
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksRequestTopic, OnPendingTasksRequestReceived);
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SelectedTaskTopic, OnTaskSelectedReceived); // 👈
    }

    private void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksRequestTopic);
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.SelectedTaskTopic); // 👈
    }


    private void OnPendingTasksRequestReceived(string _)
    {
        Debug.Log("📨 Petición de tareas recibida por MQTT desde el piloto.");
        PublishPendingTasks();
    }

    private void Start()
    {
        AdjustAllTaskItems();

        foreach (TaskItemUI taskUI in contentParent.GetComponentsInChildren<TaskItemUI>())
        {
            if (taskUI == null) continue;

            var data = taskUI.TaskData;

            // Si no hay datos, se ignora
            if (data == null)
            {
                Debug.LogWarning("⚠️ Tarea sin datos. No será incluida.");
                continue;
            }

            // Si es tarea demo con datos válidos, se respeta
            if (taskUI.isDemoTask && data.assignedDrone != null)
            {
                Debug.Log($"🛡️ Tarea demo con datos preasignados en el Inspector: {data.title} ({data.status})");
            }

            if (string.IsNullOrEmpty(data.id))
            {
                data.id = Guid.NewGuid().ToString();
                Debug.Log($"🆕 ID generado para tarea: {data.title} => {data.id}");
            }

            Debug.Log($"🧪 Hash de instancia TASKLISTMANAGER: {data.title} => {data.GetHashCode()}");

            taskUI.Setup(data, this);
        }

        PublishPendingTasks();
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

    public static void SelectTask(TaskItemUI taskUI)
    {
        if (selectedTaskUI != null && selectedTaskUI != taskUI)
            selectedTaskUI.SetHighlight(false);

        selectedTaskUI = taskUI;
        selectedTaskUI.SetHighlight(true);

        var taskData = selectedTaskUI.TaskData;
        Debug.Log($"📋 Status de la tarea: {taskData?.status}");
        Debug.Log($"📌 HashCode de TaskData: {taskData.GetHashCode()}");

        if (taskData != null && taskData.assignedDrone != null && taskData.status.Trim().ToLowerInvariant().StartsWith("executing"))
        {
            string droneId = taskData.assignedDrone.droneName;
            Debug.Log($"🔁 Mostrando vista para dron asignado: {droneId}");
            DroneViewPanelManager.ShowAllViews(droneId);
        }
        else
        {
            Debug.Log("ℹ️ Tarea seleccionada sin dron asignado o sin estado ejecutando.");
        }
    }

    private void OnTaskSelectedReceived(string json)
    {
        TaskSelectionMessage msg = JsonUtility.FromJson<TaskSelectionMessage>(json);
        if (msg == null) { Debug.LogWarning("❌ JSON de selección malformado"); return; }

        // 1️⃣ Buscar la TaskData en el listado local
        foreach (Transform child in contentParent)
        {
            TaskItemUI ui = child.GetComponent<TaskItemUI>();
            if (ui != null && ui.TaskData != null && ui.TaskData.id == msg.taskId)
            {
                // 2️⃣ Actualizar estado y refrescar UI
                ui.TaskData.status = msg.newStatus;
                ui.Setup(ui.TaskData, this);

                // 3️⃣ Auto-selección para que se abra la cámara
                SelectTask(ui);

                Debug.Log($"✅ Tarea {ui.TaskData.title} → estado '{msg.newStatus}'");
                break;
            }
        }

        // 4️⃣ Re-publicar snapshot si quieres sincronizar con otros comandantes
        PublishPendingTasks();
    }

}
