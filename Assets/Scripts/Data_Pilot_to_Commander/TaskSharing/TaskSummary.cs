[System.Serializable]
public class TaskSummary
{
    public string id;
    public string title;
    public string description;
    public string status;
    public string drone;

    public DroneData assignedDrone; // ✅ Solo si lo usas desde el subscriber

    public TaskSummary(TaskData data)
    {
        id = data.id;
        title = data.title;
        description = data.description;
        status = data.status;
        drone = data.assignedDrone != null ? data.assignedDrone.droneName : "N/A";
        assignedDrone = data.assignedDrone;
    }
}
