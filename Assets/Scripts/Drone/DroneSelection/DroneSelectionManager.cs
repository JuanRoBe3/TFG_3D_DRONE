using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneSelectionManager : MonoBehaviour
{
    [Header("Drones Available")]
    public List<DroneData> availableDrones;

    private DroneData selectedDrone;

    // ✅ Singleton Instance
    public static DroneSelectionManager Instance { get; private set; }

    // ✅ Evento que notifica cuando cambia el dron seleccionado
    public static event Action<DroneData> OnDroneChanged;

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
        // Si quieres lanzar evento al principio (opcional)
        if (selectedDrone != null)
            OnDroneChanged?.Invoke(selectedDrone);

        // Si activas tarjetas en lugar de carrusel
        // instantiateCards();
    }

    // Este método puede llamarse desde botones de tarjeta o carrusel
    public void SetSelectedDrone(DroneData drone)
    {
        selectedDrone = drone;
        Debug.Log($"Dron seleccionado manualmente: {drone.droneName}");

        OnDroneChanged?.Invoke(drone); // 🔔 Dispara evento a quien escuche
    }

    public DroneData GetSelectedDrone()
    {
        return selectedDrone;
    }

    // Solo si haces tarjetas de dron
    /*
    [Header("Drone Card Settings")]
    public Transform cardContainer;
    public DroneCardUI cardPrefab;

    private void instantiateCards()
    {
        foreach (var drone in availableDrones)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup(drone, OnDroneSelected);
        }
    }
    */
}
