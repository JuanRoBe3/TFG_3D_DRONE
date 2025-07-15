using UnityEngine;
using UnityEngine.UI;

public class TargetReplicaSelector : MonoBehaviour
{
    [Header("Cámara del comandante")]
    [SerializeField] private Camera commanderCam;

    [Header("RawImage del minimapa top-down (si se usa)")]
    [SerializeField] private RawImage topDownMinimapImage;

    [Header("Capa de detección de targets")]
    [SerializeField] private LayerMask targetMask;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray;

        // ¿Clic en minimapa top-down?
        if (topDownMinimapImage != null && PointerOnImage(topDownMinimapImage, out Vector2 uv))
        {
            ray = commanderCam.ViewportPointToRay(new Vector3(uv.x, uv.y, 0));
        }
        else
        {
            ray = commanderCam.ScreenPointToRay(Input.mousePosition);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, 5000f, targetMask))
        {
            var clicker = hit.collider.GetComponent<ClickableTarget>();
            if (clicker != null)
            {
                string id = clicker.GetTargetId();
                Debug.Log($"🎯 Click sobre target '{id}' ({hit.collider.name})");

                clicker.TriggerZoomView();
            }
            else Debug.Log("⚠️ Collider sin ClickableTarget.");
        }
        else Debug.Log("❌ Raycast no tocó ningún target.");
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
