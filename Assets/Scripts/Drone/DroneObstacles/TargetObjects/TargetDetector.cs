using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TargetDetector : MonoBehaviour
{
    private const float maxDetectionDistance = 10000f;

    public TargetPopupUI popupUI;
    public RectTransform visorRect;
    private TargetData[] allTargets;
    private TargetData currentDetectedTarget;

    [Header("Modo automático (interno, no editable)")]
    private bool automaticMode = false;

    // Referencias asignadas dinámicamente
    private Camera midZoomCamera;

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

        // Activar la cámara de zoom desde el principio
        if (midZoomCamera != null)
            midZoomCamera.gameObject.SetActive(true);
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
            }
        }
        else
        {
            HandleManualActivationInput();

            if (currentDetectedTarget == null)
            {
                popupUI.Hide();
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
        MoveZoomCameraToTarget(currentDetectedTarget);
    }

    private void MoveZoomCameraToTarget(TargetData target)
    {
        if (midZoomCamera == null) return;

        Vector3 dronePos = transform.position;
        Vector3 targetPos = target.transform.position;

        Vector3 midPoint = Vector3.Lerp(dronePos, targetPos, 0.5f); // 🔍 Zoom al 50%
        midZoomCamera.transform.position = midPoint;
        midZoomCamera.transform.LookAt(targetPos);

        float distance = Vector3.Distance(dronePos, targetPos);
        midZoomCamera.fieldOfView = Mathf.Clamp(60f * (distance / 50f), 20f, 60f);
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

        Bounds bounds = rend.bounds;

        Vector3[] pointsToCheck = {
            bounds.center,
            bounds.min,
            bounds.max,
            bounds.min + new Vector3(bounds.size.x, 0, 0),
            bounds.min + new Vector3(0, bounds.size.y, 0),
            bounds.min + new Vector3(0, 0, bounds.size.z)
        };

        foreach (var point in pointsToCheck)
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(point);
            if (screenPoint.z < 0) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(visorRect, screenPoint, cam))
            {
                float distance = Vector3.Distance(transform.position, point);
                if (distance <= maxDetectionDistance)
                    return true;
            }
        }

        return false;
    }

    private string GetCardinalDirection(float yaw)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = Mathf.RoundToInt(yaw / 45f) % 8;
        return directions[index];
    }
}
