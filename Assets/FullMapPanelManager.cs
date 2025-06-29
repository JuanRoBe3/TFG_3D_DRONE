using UnityEngine;

public class FullMapPanelManager : MonoBehaviour
{
    [SerializeField] private CommanderCameraConfigurator camConfig;
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomStep = 0.10f;
    [SerializeField] private float zoomSmooth = 0.12f;

    private float minZoom = 20f;
    private float maxZoom;
    private Camera cam;
    private bool isOrtho = true;
    private float targetOrtho;
    private Vector3 dragOrigin;

    void Awake()
    {
        cam = camConfig.GetComponent<Camera>();
        targetOrtho = cam.orthographicSize;
        maxZoom = targetOrtho * 3f;
    }

    void OnEnable()
    {
        isOrtho = true;
        cam.orthographic = true;
        camConfig.ResetToDefaultView();
        targetOrtho = cam.orthographicSize;
    }

    void OnDisable()
    {
        camConfig.ResetToDefaultView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleProjection();

        if (isOrtho) HandleOrtho();
        else HandlePerspective();
    }

    void ToggleProjection()
    {
        isOrtho = !isOrtho;
        cam.orthographic = isOrtho;

        if (isOrtho)
        {
            camConfig.ResetToDefaultView();
            targetOrtho = cam.orthographicSize;
        }
        else
        {
            Vector3 pivot = WorldBounds.Value.center;
            float dist = WorldBounds.Value.extents.magnitude * 1.4f;
            cam.transform.position = pivot + new Vector3(-dist, dist, -dist);
            cam.transform.LookAt(pivot);
        }
    }

    void HandleOrtho()
    {
        if (Input.GetMouseButtonDown(0)) dragOrigin = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 d = Input.mousePosition - dragOrigin;
            Vector3 move = new Vector3(-d.x, 0, -d.y) * panSpeed * cam.orthographicSize * 0.01f * Time.deltaTime;
            cam.transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            targetOrtho *= (scroll > 0) ? (1f - zoomStep) : (1f + zoomStep);
            targetOrtho = Mathf.Clamp(targetOrtho, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrtho, zoomSmooth);
    }

    void HandlePerspective()
    {
        if (Input.GetMouseButton(0))
        {
            float h = Input.GetAxis("Mouse X") * 3f;
            float v = -Input.GetAxis("Mouse Y") * 3f;
            cam.transform.RotateAround(WorldBounds.Value.center, Vector3.up, h);
            cam.transform.RotateAround(WorldBounds.Value.center, cam.transform.right, v);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        /*
            if (Mathf.Abs(scroll) > 0.001f)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                bool hitDetected = Physics.Raycast(ray, out RaycastHit hit);

                Vector3 targetPoint;
                if (hitDetected)
                {
                    Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 2f);
                    Debug.Log("🎯 Impacto en: " + hit.point);
                    targetPoint = hit.point;
                }
                else
                {
                    // Si no hay impacto, proyecta un punto a 100 unidades al frente
                    targetPoint = cam.transform.position + cam.transform.forward * 100f;
                }

                Vector3 direction = (targetPoint - cam.transform.position).normalized;
                float distance = Vector3.Distance(cam.transform.position, targetPoint);
                float zoomSpeed = Mathf.Clamp(distance * 500f, 1000f, 100000f);
                cam.transform.position += direction * scroll * zoomSpeed * Time.deltaTime;

                // Opcional: no acercarse demasiado si hay impacto
                if (hitDetected && Vector3.Distance(cam.transform.position, targetPoint) < 5f)
                {
                    cam.transform.position = targetPoint - direction * 5f;
                }
            }
        */

        // 🔁 ALTERNATIVA 1: Zoom hacia el centro del mapa (con velocidad adaptativa)
        if (Mathf.Abs(scroll) > 0.001f)
        {
            // 🔁 Zoom hacia el centro del mapa (con velocidad adaptativa)
            Vector3 centerDir = (WorldBounds.Value.center - cam.transform.position).normalized;
            float distToCenter = Vector3.Distance(cam.transform.position, WorldBounds.Value.center);
            float zoomSpeed = Mathf.Clamp(distToCenter * 500f, 1000f, 100000f);
            cam.transform.position += centerDir * scroll * zoomSpeed * Time.deltaTime;
        }
    }
}
