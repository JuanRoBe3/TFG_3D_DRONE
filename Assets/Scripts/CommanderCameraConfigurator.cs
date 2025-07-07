using UnityEngine;

/// <summary>
/// Coloca la cámara ortográfica centrada sobre todo el mapa
/// y guarda la posición/zoom para poder restaurarlos.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CommanderCameraConfigurator : MonoBehaviour
{
    [SerializeField] private float heightOffset = 500f;   // Altura sobre el terreno

    private Vector3 initialPosition;
    private float initialOrthoSize;
    private Quaternion initialRotation;   // ⬅️  nuevo
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        Bounds map = WorldBounds.Value;
        Vector3 c = map.center;

        cam.transform.position = new Vector3(c.x, map.max.y + heightOffset, c.z);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        float sizeNeeded = Mathf.Max(map.extents.z, map.extents.x / cam.aspect);
        cam.orthographic = true;
        cam.orthographicSize = sizeNeeded;

        // 🔐  guardar estado completo
        initialPosition = cam.transform.position;
        initialOrthoSize = cam.orthographicSize;
        initialRotation = cam.transform.rotation;   // ⬅️  nuevo
    }

    public void ResetToDefaultView()
    {
        if (cam == null) cam = GetComponent<Camera>();
        cam.transform.position = initialPosition;
        cam.transform.rotation = initialRotation;   // ⬅️  nuevo
        cam.orthographic = true;              // por si alguien lo cambió
        cam.orthographicSize = initialOrthoSize;
    }
}
