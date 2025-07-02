using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskData
{
    public string id;
    public string title;
    public string description;
    public string status;
    public DroneData assignedDrone;
    public List<Target> assignedTargets = new();

    // ✅ Constructor con nombres correctos
    public TaskData() { }  // ← Esto evita el error
    public TaskData(string title, string description, string status = "To be executed", DroneData assignedDrone = null)
    {
        this.id = Guid.NewGuid().ToString();
        this.title = title;
        this.description = description;
        this.status = status;
        this.assignedDrone = assignedDrone;
    }

    public override string ToString()
    {
        return $"[TaskData] {title} | {status} | Drone: {(assignedDrone != null ? assignedDrone.droneName : "Ninguno")}";
    }
}
