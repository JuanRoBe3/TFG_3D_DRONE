using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneCarouselUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image displayImage;
    public TMP_Text droneNameText;
    public TMP_Text storageText;
    public TMP_Text durationText;
    public TMP_Text batteryText;
    public TMP_Text rangeText;

    /// <summary>
    /// Muestra la información del dron en el carrusel, solo datos estáticos.
    /// </summary>
    public void DisplayDrone(DroneData droneData)
    {
        if (droneData == null) return;

        displayImage.sprite = droneData.icon;
        droneNameText.text = droneData.droneName;
        storageText.text = $"{droneData.storageCapacityMB} MB";
        durationText.text = $"{(droneData.estimatedFlightDurationMinutes):0} min";
        batteryText.text = $"{droneData.maxBattery:0}%";
        rangeText.text = $"{droneData.maxRange} m";
    }
}
