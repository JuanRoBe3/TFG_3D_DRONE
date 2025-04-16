using UnityEngine;

[CreateAssetMenu(fileName = "NewDroneData", menuName = "Drone/Drone Data")]
public class DroneData : ScriptableObject
{
    public string droneName;
    public string assetBundleName; // USE "droneXbundle" WHERE X is the nº of the drone (new one please)
    public Sprite icon;
    public float maxBattery = 100f;
    public float maxRange = 500f;
    public float storageCapacityMB = 1024f;
    public float estimatedFlightDurationMinutes = 1200f;
    public GameObject prefab;
}
