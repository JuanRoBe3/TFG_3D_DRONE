using UnityEngine;

[System.Serializable]
public class DroneRuntimeStatus
{
    public float currentBattery;
    public float currentStorageUsed;
    public float currentFlightTime;
    public float currentAltitude;
    public Vector3 position;
    public Quaternion rotation;
}
