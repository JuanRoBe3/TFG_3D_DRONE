using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DroneSelectionUI : MonoBehaviour
{
    [Header("Drone Info List")]
    public List<DroneData> droneModels;

    [Header("UI Prefabs and Parents")]
    public GameObject droneCardPrefab; // Prefab visual de la tarjeta
    public Transform cardParent;       // Contenedor donde se instancian las tarjetas

    [Header("Selection")]
    public Button confirmButton;
    private DroneData selectedDrone;

    void Start()
    {
        PopulateDroneCards();
        confirmButton.interactable = false;
        confirmButton.onClick.AddListener(OnConfirmSelection);
    }

    void PopulateDroneCards()
    {
        foreach (DroneData drone in droneModels)
        {
            GameObject card = Instantiate(droneCardPrefab, cardParent);
            DroneCardUI cardUI = card.GetComponent<DroneCardUI>();
            cardUI.Setup(drone, OnDroneSelected);
        }
    }

    void OnDroneSelected(DroneData drone)
    {
        selectedDrone = drone;
        confirmButton.interactable = true;
    }

    void OnConfirmSelection()
    {
        // Aquí puedes guardar el dron seleccionado en una clase singleton, o usar PlayerPrefs si prefieres
        DroneSelectionManager.Instance.SetSelectedDrone(selectedDrone);
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneConstants.PilotUI1); // o tu siguiente escena
    }
}
