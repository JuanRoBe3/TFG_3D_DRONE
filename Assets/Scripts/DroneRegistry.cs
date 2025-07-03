using System.Collections.Generic;
using UnityEngine;

public static class DroneRegistry
{
    private static Dictionary<string, DroneData> dronesByName = new();

    public static void RegisterAll(List<DroneData> drones)
    {
        dronesByName.Clear();

        foreach (var drone in drones)
        {
            if (drone != null && !string.IsNullOrEmpty(drone.droneName))
            {
                if (!dronesByName.ContainsKey(drone.droneName))
                    dronesByName.Add(drone.droneName, drone);
                else
                    Debug.LogWarning($"⚠️ Dron duplicado no registrado: {drone.droneName}");
            }
        }

        Debug.Log($"📘 DroneRegistry cargado con {dronesByName.Count} drones");
    }

    public static DroneData Get(string droneName)
    {
        if (dronesByName.TryGetValue(droneName, out var drone))
            return drone;

        Debug.LogWarning($"🔍 Dron no encontrado en DroneRegistry: {droneName}");
        return null;
    }

    public static DroneData GetDroneById(string droneId)
    {
        // ✅ El droneId es realmente el droneName
        return Get(droneId);
    }

    public static IEnumerable<DroneData> GetAll()
    {
        return dronesByName.Values;
    }
}
