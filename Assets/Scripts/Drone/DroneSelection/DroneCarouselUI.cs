using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneCarouselUI : MonoBehaviour
{
    public Image displayImage;
    public TMP_Text droneNameText;
    public TMP_Text storageText;
    public TMP_Text durationText;
    public TMP_Text batteryText;
    public TMP_Text rangeText;

    public void DisplayDrone(DroneModelInfo drone)
    {
        if (drone == null) return;

        displayImage.sprite = drone.droneImage;
        droneNameText.text = drone.droneName;
        storageText.text = $"{drone.storageGB} GB";
        durationText.text = $"{drone.maxDurationMinutes} min";
        batteryText.text = $"{drone.runtimeStats.batteryPercent}%";
        rangeText.text = $"{drone.rangeMeters} m";
    }
}
