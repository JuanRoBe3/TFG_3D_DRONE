using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TaskItemUI : MonoBehaviour
{
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescription;
    public TextMeshProUGUI statusText;
    public Image taskStatusColor; // Asignado desde el Inspector
    public TextMeshProUGUI droneNameText;
    public Image droneIcon;
    public Button editButton;

    [SerializeField] private TaskData taskData; // 🔐 Ahora privado pero serializable
    public TaskData TaskData => taskData;

    private TaskListManager taskListManager;

    // ✅ Método principal para instancias nuevas
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

    // ✅ Método auxiliar para tareas ya existentes //ya no esnecesario porque se actualiza todo
    /*
    public void BindManager(TaskListManager managerRef)
    {
        taskListManager = managerRef;
        BindEditButton(); // Solo el botón
    } 
     */

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
}
