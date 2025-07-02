using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detecta clicks sobre drones (réplicas o dummies).
/// Abre el panel correspondiente según si el clic es en minimapa top-down o en escena normal.
/// </summary>
public class DroneReplicaSelector : MonoBehaviour
{
    [Header("Cámara del comandante")]
    [SerializeField] private Camera commanderCam;

    [Header("RawImage del minimapa top-down (si se usa)")]
    [SerializeField] private RawImage topDownMinimapImage;

    [Header("Capa de detección de drones")]
    [SerializeField] private LayerMask droneMask;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray;
        DroneViewPanelManager.DisplayTarget target;

        // ¿Clic en minimapa top-down?
        if (topDownMinimapImage != null && PointerOnImage(topDownMinimapImage, out Vector2 uv))
        {
            ray = commanderCam.ViewportPointToRay(new Vector3(uv.x, uv.y, 0));
            target = DroneViewPanelManager.DisplayTarget.MinimapTop;
        }
        else
        {
            ray = commanderCam.ScreenPointToRay(Input.mousePosition);
            target = DroneViewPanelManager.DisplayTarget.MainMini;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, 5000f, droneMask))
        {
            var clicker = hit.collider.GetComponent<ClickableDrone>();
            if (clicker != null)
            {
                string id = clicker.GetDroneId();
                Debug.Log($"🚁 Click sobre dron '{id}' ({hit.collider.name})");

                DroneViewPanelManager.ShowDrone(id, target);
                clicker.TriggerSelection();  // si tienes lógica adicional
            }
            else Debug.Log("⚠️ Collider sin ClickableDrone.");
        }
        else Debug.Log("❌ Raycast no tocó ningún dron.");
    }

    private bool PointerOnImage(RawImage img, out Vector2 uv)
    {
        uv = Vector2.zero;
        RectTransform rt = img.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out Vector2 local))
            return false;

        uv.x = Mathf.InverseLerp(-rt.rect.width / 2, rt.rect.width / 2, local.x);
        uv.y = Mathf.InverseLerp(-rt.rect.height / 2, rt.rect.height / 2, local.y);

        return uv.x is >= 0f and <= 1f && uv.y is >= 0f and <= 1f;
    }
}
