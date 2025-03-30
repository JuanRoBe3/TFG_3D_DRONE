using UnityEngine;

[System.Serializable]
public class DroneModelInfo
{
    public string droneName;
    public Sprite droneImage;
    public float maxDurationMinutes;
    public float storageGB;
    public float rangeMeters;

    public DroneRuntimeStats runtimeStats;

    public string bundleName;
    public string assetName;
}
