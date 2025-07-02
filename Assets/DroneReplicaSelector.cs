using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Selecciona cualquier dron réplica (dummy o instanciado) con un clic.
/// - Si el clic ocurre dentro de un RawImage (por ejemplo tu minimapa 2-D),
///   convierte el punto de pantalla a UV y dispara el rayo desde la misma cámara.
/// - Fuera de esa imagen, dispara un ScreenPointToRay normal.
/// </summary>
public class DroneReplicaSelector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera commanderCam;        // Cámara que ve la escena (Main)
    [SerializeField] private RawImage optionalMapImage;  // Deja null si no lo necesitas
    [SerializeField] private LayerMask droneMask;        // Sólo capas donde están los drones

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;        // clic izquierdo

        Ray ray;

        // ─────────────────────────────────────────────────────────
        // ¿Hemos clicado dentro del RawImage (minimapa)?
        // ─────────────────────────────────────────────────────────
        if (optionalMapImage != null && PointerOnImage(optionalMapImage, out Vector2 uv))
        {
            // uv ∈ [0,1]²  →  ViewportPointToRay
            ray = commanderCam.ViewportPointToRay(new Vector3(uv.x, uv.y, 0));
        }
        else
        {
            // Clic normal en la pantalla
            ray = commanderCam.ScreenPointToRay(Input.mousePosition);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, 5000f, droneMask))
        {
            var clicker = hit.collider.GetComponent<ClickableDrone>();
            if (clicker != null)
            {
                Debug.Log($"🚁 Click sobre dron '{clicker.GetDroneId()}'  ({hit.collider.name})");
                clicker.TriggerSelection();              // abre el panel
            }
            else
            {
                Debug.Log("⚠️ El collider no tiene ClickableDrone.");
            }
        }
        else
        {
            Debug.Log("❌ Raycast no golpeó nada (ni dummies ni réplicas).");
        }
    }

    /// <summary>
    /// Devuelve true si el puntero está dentro del RawImage y saca sus coordenadas UV (0-1).
    /// </summary>
    private bool PointerOnImage(RawImage img, out Vector2 uv)
    {
        uv = Vector2.zero;
        RectTransform rt = img.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out Vector2 local))
            return false;

        uv.x = Mathf.InverseLerp(-rt.rect.width * 0.5f, rt.rect.width * 0.5f, local.x);
        uv.y = Mathf.InverseLerp(-rt.rect.height * 0.5f, rt.rect.height * 0.5f, local.y);

        return uv.x is >= 0f and <= 1f && uv.y is >= 0f and <= 1f;
    }
}
