using UnityEngine;

//CREO QUE ESTE NO SE USA EN TODO EL PROYECTO

[System.Serializable]
public class DroneSelectionMessage
{
    public string name;                     // Nombre del dron
    public float batteryLevel;              // Nivel de bater�a en %
    public float estimatedFlightDurationMin; // Duraci�n estimada en minutos
    public float storageCapacityMB;         // Capacidad de almacenamiento en MB
    public float maxRange;                  // Alcance m�ximo en metros
    public Sprite icon;                     // (Opcional) �cono del dron

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
