using UnityEngine;

public class DroneHUDWarning : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // Asignar el CanvasGroup del panel rojo
    public float fadeSpeed = 3f;    // Velocidad del fundido (más bajo = más lento)

    void Start()
    {
        SetAlpha(0f);
    }

    /// <summary>
    /// Ajusta progresivamente la visibilidad del HUD en función de la intensidad deseada (0 a 1).
    /// </summary>
    /// <param name="intensity">Cercanía a un obstáculo. 0 = lejos, 1 = muy cerca.</param>
    public void SetWarningIntensity(float intensity)
    {
        if (canvasGroup == null) return;

        float targetAlpha = Mathf.Clamp01(intensity);
        float newAlpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(newAlpha);
    }

    private void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
