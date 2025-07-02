using System;

[Serializable]
public class TaskSelectionMessage
{
    public string taskId;
    public string droneId;      // redundante, pero facilita debug y filtrado si quieres
    public string newStatus;    // "Executing"
}