using UnityEngine;

public class DroneVisualWarningManager : MonoBehaviour
{
    [Header("Partes visuales del dron")]
    public GameObject leftWarningObject;
    public GameObject rightWarningObject;
    public GameObject backWarningObject;

    [Header("Umbral de activación visual")]
    [Range(0f, 1f)] public float activationThreshold = 0.2f;

    public void UpdateVisuals(float leftIntensity, float rightIntensity, float backIntensity)
    {
        if (leftWarningObject != null)
            leftWarningObject.SetActive(leftIntensity > activationThreshold);

        if (rightWarningObject != null)
            rightWarningObject.SetActive(rightIntensity > activationThreshold);

        if (backWarningObject != null)
            backWarningObject.SetActive(backIntensity > activationThreshold);
    }
}
