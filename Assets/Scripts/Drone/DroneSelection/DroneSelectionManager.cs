using System.Collections.Generic;
using UnityEngine;

public class DroneSelectionManager : MonoBehaviour
{
    //SOLO UTIL SI VOY A HACER CARTAS EN VEZ DE CAROUSEL
    /*
    [Header("Drone Card Settings")]
    public Transform cardContainer; // El contenedor donde se instancian las tarjetas
    public DroneCardUI cardPrefab;  // Prefab de la tarjeta 
    */

    [Header("Drones Available")]
    public List<DroneData> availableDrones; // Aquí arrastras los datos desde el Inspector

    private DroneData selectedDrone;

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
        //para hacer tarjetas en vez de carousel (mostrar varios drones a la vez)
        //////////////////////////////
        //instantiateCards();
        //////////////////////////////
    }

    // ✅ Método llamado por las tarjetas al pulsar "Select"
    void OnDroneSelected(DroneData drone)
    {
        SetSelectedDrone(drone);
    }

    // ✅ Lo que te faltaba: lo puedes usar desde cualquier script externo
    public void SetSelectedDrone(DroneData drone)
    {
        selectedDrone = drone;
        Debug.Log($"Dron seleccionado manualmente: {drone.droneName}");
    }

    public DroneData GetSelectedDrone()
    {
        return selectedDrone;
    }

    //METODO PARA HACER TARJETAS EN VEZ DE CAROUSEL
    /*
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
