using System.Collections.Generic;
using UnityEngine;

public class DroneSelectionManager : MonoBehaviour
{
    [Header("Drone Card Settings")]
    public Transform cardContainer; // El contenedor donde se instancian las tarjetas
    public DroneCardUI cardPrefab;  // Prefab de la tarjeta

    [Header("Drones Available")]
    public List<DroneModelInfo> availableDrones; // Aquí arrastras los datos desde el Inspector

    private DroneModelInfo selectedDrone;

    // ✅ Singleton Instance
    public static DroneSelectionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        /*
         foreach (var drone in availableDrones)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup(drone, OnDroneSelected);
        }
         */
    }

    // ✅ Método llamado por las tarjetas al pulsar "Select"
    void OnDroneSelected(DroneModelInfo drone)
    {
        SetSelectedDrone(drone);
    }

    // ✅ Lo que te faltaba: lo puedes usar desde cualquier script externo
    public void SetSelectedDrone(DroneModelInfo drone)
    {
        selectedDrone = drone;
        Debug.Log($"Dron seleccionado manualmente: {drone.droneName}");
    }

    public DroneModelInfo GetSelectedDrone()
    {
        return selectedDrone;
    }
}
