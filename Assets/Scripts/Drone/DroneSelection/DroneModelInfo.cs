using UnityEngine;

//Creo que esta ya no sirve para nada

[System.Serializable]
public class DroneModelInfo
{
    public string droneName;
    public Sprite droneImage;
    public float maxDurationMinutes;
    public float storageGB;
    public float rangeMeters;

    public DroneRuntimeStatus runtimeStatus;

    public string bundleName;
    public string assetName;
}
