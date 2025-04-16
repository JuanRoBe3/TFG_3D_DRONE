public static class SelectedDroneHolder
{
    private static DroneData selectedDrone;

    public static void SetDrone(DroneData drone)
    {
        selectedDrone = drone;
    }

    public static DroneData GetDrone()
    {
        return selectedDrone;
    }
}
