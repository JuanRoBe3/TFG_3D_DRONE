using UnityEngine;
using UnityEngine.UI;
using TMPro;

//NO SIRVE DE NADA SI NO HAGO TARJETAS SINO QUE LO DEJO MODO CAROUSEL

public class DroneCardUI : MonoBehaviour
{
    public Image droneImage;
    public TMP_Text droneNameText;
    public TMP_Text durationText;
    public TMP_Text storageText;
    public TMP_Text rangeText;
    public TMP_Text batteryText;
    public Button selectButton;

    private DroneData droneData;
    private System.Action<DroneData> onSelectedCallback;

    public void Setup(DroneData data, System.Action<DroneData> onSelected)
    {
        droneData = data;
        onSelectedCallback = onSelected;

        droneImage.sprite = data.icon;
        droneNameText.text = data.droneName;
        durationText.text = $"{(data.estimatedFlightDurationMinutes):0} min";
        storageText.text = $"{data.storageCapacityMB} MB";
        rangeText.text = $"{data.maxRange} m";
        batteryText.text = $"{data.maxBattery}%";

        selectButton.onClick.AddListener(() => onSelectedCallback?.Invoke(droneData));
    }
}
