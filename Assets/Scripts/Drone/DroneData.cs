using UnityEngine;

public class DroneData : MonoBehaviour
{
    public float batteryLevel = 100f;         // %
    public float maxRange = 500f;             // metros
    public float storageAvailableMB = 1024f;  // MB
    public float flightDurationSeconds = 0f;  // tiempo actual de vuelo o restante
}
