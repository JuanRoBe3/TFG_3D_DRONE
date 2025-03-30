public static class SelectedDroneHolder
{
    private static DroneModelInfo selectedDrone;

    public static void SetDrone(DroneModelInfo drone)
    {
        selectedDrone = drone;
    }

    public static DroneModelInfo GetDrone()
    {
        return selectedDrone;
    }
}
