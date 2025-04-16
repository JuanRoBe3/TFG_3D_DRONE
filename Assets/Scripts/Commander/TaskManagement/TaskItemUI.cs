using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI droneNameText;
    public Image droneIcon;

    public Button editButton;

    private TaskData currentData;
    private TaskListManager taskListManager;

    public void Setup(TaskData data, TaskListManager managerRef)
    {
        currentData = data;
        taskListManager = managerRef;

        taskNameText.text = data.title;
        statusText.text = data.status;

        if (data.assignedDrone != null)
        {
            droneNameText.text = data.assignedDrone.droneName;

            if (data.assignedDrone.icon != null)
            {
                droneIcon.sprite = data.assignedDrone.icon;
                droneIcon.enabled = true;
            }
            else
            {
                droneIcon.enabled = false;
                Debug.LogWarning($"⚠️ El dron '{data.assignedDrone.droneName}' no tiene icono asignado.");
            }
        }
        else
        {
            droneNameText.text = "Sin dron";
            droneIcon.enabled = false;
        }

        if (editButton != null)
            editButton.onClick.AddListener(OnEditButtonClicked);
    }

    private void OnEditButtonClicked()
    {
        taskListManager.EditTask(currentData, this);
    }
}
