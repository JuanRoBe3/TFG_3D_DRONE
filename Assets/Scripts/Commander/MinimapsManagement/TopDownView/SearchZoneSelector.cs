using UnityEngine;
using UnityEngine.UI;

public class SearchZoneSelector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera topDownCamera;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private LineRenderer previewLine;
    [SerializeField] private RawImage minimapImage;

    [Header("Parámetros")]
    [Tooltip("Metros extra por encima de la cima más alta del terreno")]
    [SerializeField] private float extraHeight = 10f;

    [Tooltip("Tamaño mínimo lateral (m)")]
    [SerializeField] private float minSizeMeters = 10f;

    private bool dragging = false;
    private bool hasPending = false;
    private Vector3 startWorldPos;
    private Vector3 endWorldPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && RayToTerrain(out startWorldPos))
        {
            dragging = true;
            hasPending = false;
        }

        if (dragging && Input.GetMouseButton(1) && RayToTerrain(out endWorldPos))
        {
            DrawPreview(startWorldPos, endWorldPos);
        }

        if (dragging && Input.GetMouseButtonUp(1))
        {
            dragging = false;
            hasPending = true;
            Debug.Log("🟡 Zona pendiente. Pulsa Confirm para crearla.");
        }
    }

    public void ConfirmZone()
    {
        if (!hasPending)
        {
            Debug.LogWarning("❌ No hay zona pendiente.");
            return;
        }

        Vector3 min = Vector3.Min(startWorldPos, endWorldPos);
        Vector3 max = Vector3.Max(startWorldPos, endWorldPos);

        float width = max.x - min.x;
        float depth = max.z - min.z;

        if (width < minSizeMeters || depth < minSizeMeters)
        {
            Debug.LogWarning("❌ Selección demasiado pequeña.");
            return;
        }

        float terrainMaxY = WorldBounds.Value.max.y;
        float height = terrainMaxY + extraHeight;

        Vector3 center = new Vector3(
            (min.x + max.x) / 2f,
            height / 2f,
            (min.z + max.z) / 2f
        );

        Vector3 size = new Vector3(width, height, depth);

        GameObject zone = Instantiate(zonePrefab, center, Quaternion.identity);
        zone.transform.localScale = size;

        Debug.Log($"📏 Tamaño zona calculado: {size}");
        Debug.Log($"📦 Escala aplicada: {zone.transform.localScale}");

        var publisher = FindObjectOfType<SearchZonePublisherManager>();
        if (publisher != null)
        {
            publisher.RegisterZone(new SearchZoneData
            {
                center = new SerializableVector3(center),
                size = new SerializableVector3(size)
            });
        }
        else
        {
            Debug.LogError("❌ No se encontró SearchZonePublisherManager en escena.");
        }

        previewLine.enabled = false;
        hasPending = false;

        Debug.Log("✅ Zona creada y publicada por MQTT.");

        // Llama directamente a la generación de la ruta para esta zona
        SearchRouteGenerator routeGenerator = FindObjectOfType<SearchRouteGenerator>();
        if (routeGenerator != null)
        {
            routeGenerator.GenerateRouteForZone(zone);  // 🔸 LLAMA A VERSIÓN PÚBLICA (ver abajo)
        }
        else
        {
            Debug.LogError("❌ No se encontró SearchRouteGenerator en la escena.");
        }

    }

    private bool RayToTerrain(out Vector3 hit)
    {
        hit = Vector3.zero;

        RectTransform rt = minimapImage.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out Vector2 local))
            return false;

        Vector2 uv = new Vector2(
            Mathf.InverseLerp(-rt.rect.width * 0.5f, rt.rect.width * 0.5f, local.x),
            Mathf.InverseLerp(-rt.rect.height * 0.5f, rt.rect.height * 0.5f, local.y));

        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            return false;

        Vector3 viewport = new Vector3(uv.x, uv.y, 0);
        Ray ray = topDownCamera.ViewportPointToRay(viewport);

        if (Physics.Raycast(ray, out RaycastHit h, 1000f, terrainMask))
        {
            hit = h.point;
            return true;
        }

        return false;
    }

    private void DrawPreview(Vector3 a, Vector3 b)
    {
        float y = WorldBounds.Value.max.y + extraHeight + 1f;

        Vector3 p0 = new Vector3(a.x, y, a.z);
        Vector3 p1 = new Vector3(a.x, y, b.z);
        Vector3 p2 = new Vector3(b.x, y, b.z);
        Vector3 p3 = new Vector3(b.x, y, a.z);

        previewLine.positionCount = 5;
        previewLine.SetPositions(new[] { p0, p1, p2, p3, p0 });
        previewLine.enabled = true;
    }
}
