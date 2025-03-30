[System.Serializable]
public class DroneSelectionMessage
{
    public string droneName;
    public float batteryPercent;
    public float maxDurationMinutes;
    public float storageGB;
    public float rangeMeters;

    public DroneSelectionMessage(DroneModelInfo drone)
    {
        droneName = drone.droneName;
        batteryPercent = drone.runtimeStats.batteryPercent;
        maxDurationMinutes = drone.maxDurationMinutes;
        storageGB = drone.storageGB;
        rangeMeters = drone.rangeMeters;
    }
}
