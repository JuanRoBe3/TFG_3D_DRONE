using UnityEngine;

/// Coloca la cámara ortográfica para abarcar todo el landscape una sola vez.
[RequireComponent(typeof(Camera))]
public class SceneMapCameraConfigurator : MonoBehaviour
{
    [SerializeField] private int padding = 20;

    void Start()
    {
        Bounds b = WorldBounds.Value;
        Camera cam = GetComponent<Camera>();
        float size = Mathf.Max(b.extents.z, b.extents.x / cam.aspect);
        cam.orthographicSize = size + padding;
        cam.transform.position = new Vector3(b.center.x, b.max.y + 50f, b.center.z);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
