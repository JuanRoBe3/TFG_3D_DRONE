using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private const float maxDetectionDistance = 10000f;

    public TargetPopupUI popupUI;
    public RectTransform visorRect;
    private TargetData[] allTargets;

    void Start()
    {
        popupUI = FindObjectOfType<TargetPopupUI>();
        if (popupUI == null)
            Debug.LogError("❌ No se encontró TargetPopupUI en la escena.");

        GameObject visorGO = GameObject.FindWithTag("TargetVisor");
        if (visorGO != null)
            visorRect = visorGO.GetComponent<RectTransform>();
        else
            Debug.LogError("❌ No se encontró ningún objeto con tag 'TargetVisor'.");

        allTargets = FindObjectsOfType<TargetData>();
    }

    void Update()
    {
        if (popupUI == null || visorRect == null) return;
        DetectVisibleTarget();
    }

    void DetectVisibleTarget()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        foreach (var target in allTargets)
        {
            if (target == null) continue;

            if (IsTargetInsideVisor(target, cam))
            {
                string dir = GetCardinalDirection(transform.eulerAngles.y);
                popupUI.ShowTargetInfo(target.targetId, dir);
                return;
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
