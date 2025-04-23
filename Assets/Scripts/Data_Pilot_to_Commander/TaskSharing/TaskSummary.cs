using UnityEngine;

[System.Serializable]
public class TaskSummary
{
    public string id;            // 🆕 Añadido: identificador único
    public string title;
    public string description;
    public string status;
    public string drone;

    public TaskSummary(TaskData data)
    {
        id = data.id; // ✅ Copiamos también el ID
        title = data.title;
        description = data.description;
        status = data.status;
        drone = data.assignedDrone != null ? data.assignedDrone.droneName : "N/A";
    }
}

