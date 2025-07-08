using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using System.Collections;

public class TargetDetector : MonoBehaviour
{
    private const float maxDetectionDistance = 10000f;

    public TargetPopupUI popupUI;
    public RectTransform visorRect;
    private TargetData[] allTargets;
    private TargetData currentDetectedTarget;

    [Header("Modo automático (interno, no editable)")]
    private bool automaticMode = false;

    private Camera midZoomCamera;
    private Coroutine zoomCoroutine;

    private bool isZoomed = false;
    private float originalFOV;

    private void Start()
    {
        popupUI = FindObjectOfType<TargetPopupUI>();
        if (popupUI == null)
            Debug.LogError("❌ No se encontró TargetPopupUI en la escena.");

        GameObject visorGO = GameObject.FindWithTag("TargetVisor");
        if (visorGO != null)
            visorRect = visorGO.GetComponent<RectTransform>();
        else
            Debug.LogError("❌ No se encontró ningún objeto con tag 'TargetVisor'.");

        GameObject camGO = GameObject.FindWithTag("MidZoomCamera");
        if (camGO != null)
            midZoomCamera = camGO.GetComponent<Camera>();
        else
            Debug.LogError("❌ No se encontró ninguna cámara con tag 'MidZoomCamera'.");

        allTargets = FindObjectsOfType<TargetData>();

        if (midZoomCamera != null)
            midZoomCamera.gameObject.SetActive(false); // Se activa solo al hacer zoom
    }

    private void Update()
    {
        if (visorRect == null || popupUI == null) return;

        DetectVisibleTarget();

        if (automaticMode)
        {
            if (currentDetectedTarget != null)
            {
                string dir = GetCardinalDirection(transform.eulerAngles.y);
                popupUI.ShowTargetInfo(currentDetectedTarget.targetId, dir);
            }
            else
            {
                popupUI.Hide();
                ResetZoomState(); // 🧠 Resetea zoom si se pierde el target
            }
        }
        else
        {
            HandleManualActivationInput();

            if (currentDetectedTarget == null)
            {
                popupUI.Hide();
                ResetZoomState(); // 🧠 También resetea en modo manual
            }
        }
    }

    private void HandleManualActivationInput()
    {
        var joystick = Joystick.current;
        if (joystick == null || popupUI == null) return;

        if (joystick.TryGetChildControl<ButtonControl>("button2")?.wasPressedThisFrame == true)
        {
            TryActivateTarget();
        }
    }

    public void TryActivateTarget()
    {
        if (currentDetectedTarget == null || popupUI == null) return;

        string dir = GetCardinalDirection(transform.eulerAngles.y);
        popupUI.ShowTargetInfo(currentDetectedTarget.targetId, dir);

        StartZoomEffect();
    }

    private void StartZoomEffect()
    {
        if (midZoomCamera == null || Camera.main == null) return;

        if (!isZoomed)
        {
            midZoomCamera.transform.position = Camera.main.transform.position;
            midZoomCamera.transform.rotation = Camera.main.transform.rotation;

            originalFOV = Camera.main.fieldOfView;

            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomToFOV(20f));
            isZoomed = true;
        }
        else
        {
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomToFOV(originalFOV, disableAfter: true));
            isZoomed = false;
        }
    }

    private IEnumerator ZoomToFOV(float targetFOV, bool disableAfter = false)
    {
        float duration = 0.4f;
        float startFOV = midZoomCamera.fieldOfView;
        float elapsed = 0f;

        midZoomCamera.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            midZoomCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            yield return null;
        }

        midZoomCamera.fieldOfView = targetFOV;

        if (disableAfter)
            midZoomCamera.gameObject.SetActive(false);
    }

    private void ResetZoomState()
    {
        isZoomed = false;

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (midZoomCamera != null)
            midZoomCamera.gameObject.SetActive(false);
    }

    private void DetectVisibleTarget()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        currentDetectedTarget = null;

        foreach (var target in allTargets)
        {
            if (target == null) continue;

            if (IsTargetInsideVisor(target, cam))
            {
                currentDetectedTarget = target;
                break;
            }
        }
    }

    private bool IsTargetInsideVisor(TargetData target, Camera cam)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null) return false;

        Vector3 screenPoint = cam.WorldToScreenPoint(rend.bounds.center);
        if (screenPoint.z < 0) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(visorRect, screenPoint, cam);
    }

    private string GetCardinalDirection(float yaw)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = Mathf.RoundToInt(yaw / 45f) % 8;
        return directions[index];
    }
}
