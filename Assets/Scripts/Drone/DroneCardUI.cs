using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DroneCardUI : MonoBehaviour
{
    public Image droneImage;
    public TMP_Text droneNameText;
    public TMP_Text durationText;
    public TMP_Text storageText;
    public TMP_Text rangeText;
    public TMP_Text batteryText;
    public Button selectButton;

    private DroneModelInfo droneInfo;
    private System.Action<DroneModelInfo> onSelectedCallback;

    public void Setup(DroneModelInfo info, System.Action<DroneModelInfo> onSelected)
    {
        droneInfo = info;
        onSelectedCallback = onSelected;

        droneImage.sprite = info.droneImage;
        droneNameText.text = info.droneName;
        durationText.text = $"{info.maxDurationMinutes} min";
        storageText.text = $"{info.storageGB} GB";
        rangeText.text = $"{info.rangeMeters} m";
        batteryText.text = $"{info.runtimeStats.batteryPercent}%";

        selectButton.onClick.AddListener(() => onSelectedCallback?.Invoke(droneInfo));
    }
}
