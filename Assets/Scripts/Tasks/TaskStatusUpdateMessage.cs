using UnityEngine;

[System.Serializable]
public class TaskStatusUpdateMessage
{
    public string taskId;
    public string droneId;
    public string newStatus;
}
