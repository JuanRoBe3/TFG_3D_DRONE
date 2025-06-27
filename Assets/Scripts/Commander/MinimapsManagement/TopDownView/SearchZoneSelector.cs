using UnityEngine;

public class SearchZoneSelector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera topDownCamera;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private LineRenderer previewLine;
    [SerializeField] private SearchZonePublisher publisher;

    [Header("Parámetros")]
    [Tooltip("Metros que sobresalen por encima de la cima más alta")]
    [SerializeField] private float extraHeight = 20f;
    [Tooltip("Tamaño mínimo lateral (m)")]
    [SerializeField] private float minSizeMeters = 10f;

    // Estado interno
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

        // Calcular dimensiones de la selección
        Vector3 min = Vector3.Min(startWorldPos, endWorldPos);
        Vector3 max = Vector3.Max(startWorldPos, endWorldPos);

        float width = max.x - min.x;
        float depth = max.z - min.z;

        if (width < minSizeMeters || depth < minSizeMeters)
        {
            Debug.LogWarning("❌ Selección demasiado pequeña.");
            return;
        }

        float yMax = WorldBounds.Value.max.y;
        float height = yMax + extraHeight;

        Vector3 center = new Vector3(
            (min.x + max.x) / 2f,
            height / 2f,
            (min.z + max.z) / 2f
        );

        Vector3 size = new Vector3(width, height, depth);

        GameObject zone = Instantiate(zonePrefab, center, Quaternion.identity);
        zone.transform.localScale = size;

        publisher.PublishZone(center, size);

        previewLine.enabled = false;
        hasPending = false;

        Debug.Log("✅ Zona creada y publicada.");
    }

    private bool RayToTerrain(out Vector3 hit)
    {
        Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit h, 1000f, terrainMask))
        {
            hit = h.point;
            return true;
        }

        hit = Vector3.zero;
        return false;
    }

    private void DrawPreview(Vector3 a, Vector3 b)
    {
        float y = WorldBounds.Value.max.y + extraHeight;

        Vector3 p0 = new Vector3(a.x, y, a.z);
        Vector3 p1 = new Vector3(a.x, y, b.z);
        Vector3 p2 = new Vector3(b.x, y, b.z);
        Vector3 p3 = new Vector3(b.x, y, a.z);

        previewLine.positionCount = 5;
        previewLine.SetPositions(new[] { p0, p1, p2, p3, p0 });
        previewLine.enabled = true;
    }
}
