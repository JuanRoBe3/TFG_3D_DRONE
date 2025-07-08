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

        ActivateVisorZoomView(currentDetectedTarget);
    }

    private void ActivateVisorZoomView(TargetData target)
    {
        if (midZoomCamera == null) return;

        // Siempre lanzamos un raycast desde el centro del visor
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Vector3 dronePos = transform.position;
        Vector3 direction = ray.direction.normalized;

        // Puedes ajustar esta distancia para cambiar el "zoom"
        float zoomDistance = Vector3.Distance(dronePos, target.transform.position);

        // Punto a donde debería mirar la cámara (aunque no haya colisión)
        Vector3 targetPoint = dronePos + direction * zoomDistance;

        // Posición a mitad de camino entre el dron y ese punto
        Vector3 midPoint = Vector3.Lerp(dronePos, targetPoint, 0.5f);

        midZoomCamera.transform.position = midPoint;
        midZoomCamera.transform.rotation = Quaternion.LookRotation(direction);

        midZoomCamera.fieldOfView = Mathf.Clamp(60f * (zoomDistance / 50f), 20f, 60f);

        Debug.Log($"🔭 Zoom activado hacia dirección del visor. Distancia: {zoomDistance:F2}");
    }


    private void FocusCameraOn(Vector3 targetPoint)
    {
        Vector3 dronePos = transform.position;
        Vector3 midPoint = Vector3.Lerp(dronePos, targetPoint, 0.5f); // Zoom al 50%

        midZoomCamera.transform.position = midPoint;
        midZoomCamera.transform.LookAt(targetPoint);

        float distance = Vector3.Distance(dronePos, targetPoint);
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
