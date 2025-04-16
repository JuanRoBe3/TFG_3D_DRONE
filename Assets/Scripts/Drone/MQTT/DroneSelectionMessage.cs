using UnityEngine;

//CREO QUE ESTE NO SE USA EN TODO EL PROYECTO

[System.Serializable]
public class DroneSelectionMessage
{
    public string name;                     // Nombre del dron
    public float batteryLevel;              // Nivel de batería en %
    public float estimatedFlightDurationMin; // Duración estimada en minutos
    public float storageCapacityMB;         // Capacidad de almacenamiento en MB
    public float maxRange;                  // Alcance máximo en metros
    public Sprite icon;                     // (Opcional) ícono del dron

    /*
    public DroneSelectionMessage(DroneData drone, DroneRuntimeStatus runtime)
    {
        name = drone.name;
        batteryLevel = runtime.currentBattery;
        estimatedFlightDurationMin = drone.estimatedFlightDurationSeconds / 60f;
        storageCapacityMB = drone.storageCapacityMB;
        maxRange = drone.maxRange;
        icon = drone.icon;
    } 
     */
}
