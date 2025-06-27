using UnityEngine;

/// <summary>
/// Ajusta la cámara top-down para que TODO el terreno
/// quepa en pantalla sin deformaciones, independientemente
/// del aspect-ratio del Game View o del RawImage.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CommanderCameraConfigurator : MonoBehaviour
{
    [Range(0f, 0.3f)]
    [SerializeField] private float extraMargin = 0.05f; // 5 %
    [SerializeField] private float heightOffset = 50f;   // m sobre la cima

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        Bounds map = WorldBounds.Value;                // tu utilidad global
        Vector3 c = map.center;

        // 1️⃣  Poner la cámara en el centro X-Z, a buena altura
        float y = map.max.y + heightOffset;
        cam.transform.position = new Vector3(c.x, y, c.z);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 2️⃣  Calcular orthoSize teniendo en cuenta el ASPECT
        float halfDepth = map.extents.z;                // eje Z (de arriba ↓)
        float halfWidth = map.extents.x;                // eje X (izq-dcha)
        float sizeNeeded = Mathf.Max(halfDepth,          // alto
                                     halfWidth / cam.aspect); // ancho ÷ aspect

        cam.orthographic = true;
        cam.orthographicSize = sizeNeeded * (1f + extraMargin);

        Debug.Log(
            $"📐 Ajuste T-Down ⇒ size={cam.orthographicSize:F1}" +
            $"  aspect={cam.aspect:F2}  visibleXZ=({halfWidth * 2:F0}×{halfDepth * 2:F0})"
        );
    }
}
