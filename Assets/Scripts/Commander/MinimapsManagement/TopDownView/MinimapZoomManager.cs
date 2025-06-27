using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinimapZoomManager : MonoBehaviour
{
    public RectTransform minimapRect;
    public CanvasGroup mainUIGroup;
    public Button searchZoneButton;
    public float zoomDuration = 0.4f;
    public Vector2 targetSize = new Vector2(800, 800); // tamaño expandido en px

    private Vector2 originalSize;
    private Vector3 originalPosition;

    void Start()
    {
        originalSize = minimapRect.sizeDelta;
        originalPosition = minimapRect.anchoredPosition;
        searchZoneButton.gameObject.SetActive(false);
    }

    public void OnMinimapClicked()
    {
        StartCoroutine(ZoomSequence());
    }

    IEnumerator ZoomSequence()
    {
        // Ocultar el resto de UI con fade
        StartCoroutine(FadeCanvasGroup(mainUIGroup, 1, 0, zoomDuration));

        // Zoom animado del minimapa
        Vector2 startSize = minimapRect.sizeDelta;
        Vector2 startPos = minimapRect.anchoredPosition;
        Vector2 endSize = targetSize;
        Vector2 endPos = Vector2.zero; // centro de pantalla

        float t = 0;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.SmoothStep(0, 1, t / zoomDuration);
            minimapRect.sizeDelta = Vector2.Lerp(startSize, endSize, lerp);
            minimapRect.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);
            yield return null;
        }

        // Mostrar el botón para activar la zona de búsqueda
        searchZoneButton.gameObject.SetActive(true);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(from, to, t / duration);
            group.alpha = lerp;
            yield return null;
        }
        group.alpha = to;
        group.interactable = to > 0;
        group.blocksRaycasts = to > 0;
    }
}
