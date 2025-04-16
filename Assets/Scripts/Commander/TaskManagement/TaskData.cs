[System.Serializable]
public class TaskData
{
    public string id;
    public string status;
    public DroneData assignedDrone; // ✅ antes era string
    public string title;
    public string description;
}
