using UnityEngine;
using UnityEngine.UI;

public class MinimapDebugHelper : MonoBehaviour
{
    [Header("Auto asignación")]
    public string droneTag = "Drone"; // Asegúrate de que tu dron tenga este tag
    public string topDownCameraName = "TopDownCamera";

    [Header("UI RawImage del minimapa")]
    public RawImage rawImageTopDownView;

    private Camera topDownCamera;

    void Start()
    {
        FindTopDownCamera();

        if (topDownCamera == null)
        {
            Debug.LogError("🚫 No se encontró la cámara cenital en el dron instanciado.");
        }
        else
        {
            Debug.Log("🎥 Cámara TopDown encontrada: " + topDownCamera.name);

            if (topDownCamera.targetTexture == null)
                Debug.LogError("❌ La cámara no tiene RenderTexture asignado.");
            else
                Debug.Log("✅ RenderTexture asignado a cámara: " + topDownCamera.targetTexture.name);
        }

        if (rawImageTopDownView == null)
        {
            Debug.LogError("🚫 RawImageTopDownView no está asignada.");
        }
        else
        {
            if (rawImageTopDownView.texture == null)
                Debug.LogWarning("⚠️ RawImage no tiene una textura asignada.");
            else
                Debug.Log("✅ RawImage tiene texture activa: " + rawImageTopDownView.texture.name);
        }
    }

    void Update()
    {
        if (topDownCamera != null && rawImageTopDownView != null)
        {
            if (rawImageTopDownView.texture != topDownCamera.targetTexture)
            {
                rawImageTopDownView.texture = topDownCamera.targetTexture;
                Debug.Log("🔄 RenderTexture sincronizada con la RawImage.");
            }

            // Dibujo de rayo
            Ray ray = new Ray(topDownCamera.transform.position, topDownCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red);
            }
        }
    }

    private void FindTopDownCamera()
    {
        GameObject drone = GameObject.FindWithTag(droneTag);
        if (drone == null)
        {
            Debug.LogError("❌ No se encontró ningún objeto con tag 'Drone'.");
            return;
        }

        Transform camTransform = drone.transform.Find("TopDownCamera");
        if (camTransform == null)
        {
            Debug.LogError("❌ No se encontró 'TopDownCamera' como hijo del dron.");
            return;
        }

        topDownCamera = camTransform.GetComponent<Camera>();
    }
}
