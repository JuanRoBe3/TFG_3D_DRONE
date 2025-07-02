// Paso 1: DroneDataValidator.cs
using System.Collections.Generic;
using UnityEngine;

public static class DroneDataValidator
{
    public static void Validate(List<DroneData> drones)
    {
        HashSet<string> seenNames = new();

        foreach (var drone in drones)
        {
            if (drone == null)
            {
                Debug.LogError("❌ Hay un DroneData null en la lista");
                continue;
            }

            if (string.IsNullOrWhiteSpace(drone.droneName))
            {
                Debug.LogError($"❌ DroneData sin nombre: {drone.name}");
                continue;
            }

            if (seenNames.Contains(drone.droneName))
            {
                Debug.LogError($"❌ Nombre duplicado: {drone.droneName}");
                continue;
            }

            seenNames.Add(drone.droneName);
        }

        Debug.Log("✅ DroneDataValidator completado. Todo correcto.");
    }
}