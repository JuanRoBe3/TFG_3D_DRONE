using UnityEngine;

public class DroneHUDWarningManager : MonoBehaviour
{
    public DroneHUDWarning leftHUD;
    public DroneHUDWarning rightHUD;
    public DroneHUDWarning backHUD;

    public void UpdateWarnings(float leftIntensity, float rightIntensity, float backIntensity)
    {
        if (leftHUD != null)
            leftHUD.SetWarningIntensity(leftIntensity);

        if (rightHUD != null)
            rightHUD.SetWarningIntensity(rightIntensity);

        if (backHUD != null)
            backHUD.SetWarningIntensity(backIntensity);
    }
}
