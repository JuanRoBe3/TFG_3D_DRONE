using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneCarouselController : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Button selectButton;

    private List<DroneModelInfo> drones;
    private int currentIndex = 0;

    [SerializeField] private DroneCarouselUI uiUpdater;

    void Start()
    {
        drones = DroneSelectionManager.Instance.availableDrones;

        if (drones == null || drones.Count == 0)
        {
            Debug.LogError("No hay drones disponibles.");
            return;
        }

        UpdateUI();

        leftButton.onClick.AddListener(GoLeft);
        rightButton.onClick.AddListener(GoRight);
        selectButton.onClick.AddListener(OnSelectClicked);
    }

    void GoLeft()
    {
        currentIndex = (currentIndex - 1 + drones.Count) % drones.Count;
        UpdateUI();
    }

    void GoRight()
    {
        currentIndex = (currentIndex + 1) % drones.Count;
        UpdateUI();
    }

    void UpdateUI()
    {
        DroneModelInfo drone = drones[currentIndex];
        DroneSelectionManager.Instance.SetSelectedDrone(drone);
        uiUpdater?.DisplayDrone(drone);
    }

    void OnSelectClicked()
    {
        DroneModelInfo selected = DroneSelectionManager.Instance.GetSelectedDrone();
        if (selected == null) return;

        SelectedDroneHolder.SetDrone(selected);

        string message =
            $"{selected.droneName};" +
            $"{selected.runtimeStats.batteryPercent};" +
            $"{selected.maxDurationMinutes};" +
            $"{selected.storageGB};" +
            $"{selected.rangeMeters}";

        /*
        MQTTPublisher.Instance.PublishMessage(
            MQTTConstants.SelectedDroneTopic,
            message
        );
        */

        SceneLoader.LoadPilotUI();
    }
}
