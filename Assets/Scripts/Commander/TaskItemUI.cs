using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI droneNameText;
    public Image droneIcon;

    public Sprite drone1Icon;
    public Sprite drone2Icon;

    public Button editButton;

    private TaskData currentData;
    private TaskListManager taskListManager;

    public void Setup(TaskData data, TaskListManager managerRef)
    {
        currentData = data;
        taskListManager = managerRef;

        taskNameText.text = data.title;
        statusText.text = data.status;
        droneNameText.text = data.assignedDrone;

        switch (data.assignedDrone)
        {
            case "Drone1": droneIcon.sprite = drone1Icon; break;
            case "Drone2": droneIcon.sprite = drone2Icon; break;
        }

        // Asignar el listener del botón de editar
        if (editButton != null)
            editButton.onClick.AddListener(OnEditButtonClicked);
    }

    private void OnEditButtonClicked()
    {
        taskListManager.EditTask(currentData, this);
    }
}
