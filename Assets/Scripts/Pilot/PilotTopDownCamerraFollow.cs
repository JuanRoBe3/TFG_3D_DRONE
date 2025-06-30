using UnityEngine;

/// Sigue al dron (XZ) y en Awake ajusta orthographicSize usando WorldBounds.
[RequireComponent(typeof(Camera))]
public class PilotTopDownCameraFollow : MonoBehaviour
{
    [SerializeField] float padding = 20f;

    Transform droneT;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        DroneLoader.OnDroneInstantiated += d => droneT = d.transform;

        // framing único al arrancar
        Bounds b = WorldBounds.Value;
        float side = Mathf.Max(b.size.x, b.size.z) + padding * 2f;
        cam.orthographicSize = side * 0.5f;
        transform.position = new Vector3(b.center.x,
                                            transform.position.y,
                                            b.center.z);
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    void OnDestroy()
    {
        DroneLoader.OnDroneInstantiated -= d => droneT = d.transform;
    }

    void LateUpdate()
    {
        if (!droneT) return;
        Vector3 p = droneT.position; p.y = transform.position.y;
        transform.position = p;
    }
}
