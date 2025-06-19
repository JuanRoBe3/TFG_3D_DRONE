using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskData
{
    public string id;
    public string status;
    public DroneData assignedDrone;
    public string title;
    public string description;

    public List<Target> assignedTargets = new();   // ← NUEVO

    /*  (opcional) método helper para búsquedas rápidas
    private HashSet<int> cache;
    public bool ContainsTarget(Target t)
    {
        cache ??= new HashSet<int>(assignedTargets.ConvertAll(o => o.GetInstanceID()));
        return cache.Contains(t.GetInstanceID());
    }*/
}
