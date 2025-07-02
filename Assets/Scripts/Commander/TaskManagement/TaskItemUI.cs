using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TaskItemUI : MonoBehaviour
{
    [SerializeField] public bool isDemoTask = false;
    [SerializeField] public GameObject selectionHighlight;
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescription;
    public TextMeshProUGUI statusText;
    public Image taskStatusColor;
    public TextMeshProUGUI droneNameText;
    public Image droneIcon;
    public Button editButton;

    [SerializeField] private TaskData taskData;
    public TaskData TaskData => taskData;

    private TaskListManager taskListManager;

    private void Awake()
    {
        // 🧠 Auto-generar TaskData si es demo y no se asignó manualmente
        if (isDemoTask && taskData == null)
        {
            taskData = new TaskData(
                title: taskNameText != null ? taskNameText.text : "Demo",
                description: taskDescription != null ? taskDescription.text : "Tarea demo sin descripción",
                status: statusText != null ? statusText.text : "To be executed"
            );

            Debug.Log($"📦 TaskData auto-generado en escena: {taskData}");
        }
    }

    public void Setup(TaskData data, TaskListManager managerRef)
    {
        if (string.IsNullOrEmpty(data.id))
        {
            data.id = Guid.NewGuid().ToString();
            Debug.Log($"🆔 Se asignó un ID automático a una tarea existente: {data.title} => {data.id}");
        }

        taskData = data;
        taskListManager = managerRef;
        UpdateVisual();
        BindEditButton();
    }

    private void BindEditButton()
    {
        if (editButton != null)
        {
            editButton.onClick.RemoveAllListeners();
            editButton.onClick.AddListener(OnEditButtonClicked);
        }
    }

    private void OnEditButtonClicked()
    {
        if (taskData != null && taskListManager != null)
            taskListManager.EditTask(taskData, this);
    }

    private void UpdateVisual()
    {
        if (taskData == null) return;

        taskNameText.text = taskData.title;
        statusText.text = taskData.status;
        taskStatusColor.color = TaskStatusColor.GetColorForStatus(taskData.status);
        taskDescription.text = taskData.description;

        if (taskData.assignedDrone != null)
        {
            droneNameText.text = taskData.assignedDrone.droneName;

            if (taskData.assignedDrone.icon != null)
            {
                droneIcon.sprite = taskData.assignedDrone.icon;
                droneIcon.enabled = true;
            }
            else
            {
                droneIcon.enabled = false;
                Debug.LogWarning($"⚠️ El dron '{taskData.assignedDrone.droneName}' no tiene icono asignado.");
            }
        }
        else
        {
            droneNameText.text = "Sin dron";
            droneIcon.enabled = false;
        }
    }

    public void OnClickOnDroneIcon()
    {
        TaskListManager.SelectTask(this);
    }

    public void SetHighlight(bool active)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(active);
    }
}
