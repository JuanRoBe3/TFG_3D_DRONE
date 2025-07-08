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

    [Header("Detección por raycast")]
    public LayerMask obstacleMask; // capas que bloquean (como terreno)

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
            midZoomCamera.gameObject.SetActive(false);  // Se activa solo al hacer zoom
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
                ResetZoomState(); // 🔁 Esto garantiza que isZoomed se vuelve false
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
        // 🔥 Confirmar el descubrimiento solo aquí
        if (!currentDetectedTarget.IsDiscovered)
        {
            TargetDiscoveryManager.Instance.HandleTargetDiscovered(
                currentDetectedTarget,
                Camera.main.transform
            );
        }
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

    private void DetectVisibleTarget()
    {
        Camera cam = Camera.main;
        if (cam == null || visorRect == null) return;

        currentDetectedTarget = null;

        int gridResolution = 8;
        float terrainLayer = LayerMask.NameToLayer("Terrain");
        int detectableLayer = LayerMask.NameToLayer("DetectableTarget");

        // 🖼️ Obtener los bordes del visor en coordenadas de pantalla
        Vector3[] corners = new Vector3[4];
        visorRect.GetWorldCorners(corners); // orden: 0=bottomLeft, 1=topLeft, 2=topRight, 3=bottomRight

        Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector3 topRight = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        foreach (var target in allTargets)
        {
            if (target == null) continue;
            bool visible = false;

            for (int x = 0; x < gridResolution && !visible; x++)
            {
                for (int y = 0; y < gridResolution && !visible; y++)
                {
                    float px = bottomLeft.x + (width * x) / (gridResolution - 1);
                    float py = bottomLeft.y + (height * y) / (gridResolution - 1);
                    Vector3 screenPoint = new Vector3(px, py, 0f);

                    Ray ray = cam.ScreenPointToRay(screenPoint);
                    RaycastHit[] hits = Physics.RaycastAll(ray, maxDetectionDistance);
                    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                    foreach (var hit in hits)
                    {
                        GameObject obj = hit.collider.gameObject;
                        int layer = obj.layer;

                        // 🎯 Target válido
                        if (layer == detectableLayer &&
                            (obj == target.gameObject || obj.transform.IsChildOf(target.transform)))
                        {
                            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.5f);
                            currentDetectedTarget = target; 
                            visible = true;
                            break;
                        }

                        // 🧱 Bloqueado por terreno u obstáculo
                        if (layer == terrainLayer || (obstacleMask.value & (1 << layer)) != 0)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 0.2f);
                            break;
                        }

                        // ⚪ Ignorar otras cosas
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 0.05f);
                    }
                }
            }

            if (currentDetectedTarget != null)
                break;
        }
    }

    private void ResetZoomState()
    {
        isZoomed = false;

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (midZoomCamera != null)
            midZoomCamera.gameObject.SetActive(false);
    }

    private string GetCardinalDirection(float yaw)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = Mathf.RoundToInt(yaw / 45f) % 8;
        return directions[index];
    }
}
