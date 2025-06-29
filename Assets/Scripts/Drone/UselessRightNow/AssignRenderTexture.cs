using UnityEngine;

/// <summary>
/// Asigna el RenderTexture a la cámara. Se ejecuta antes que otros configuradores.
/// </summary>
[DefaultExecutionOrder(-100)]  // ⬅️ esto garantiza que se ejecute ANTES
public class AssignRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null && renderTexture != null)
        {
            cam.targetTexture = renderTexture;
            Debug.Log($"✅ RenderTexture asignado a {cam.name}");
        }
        else
        {
            Debug.LogError("❌ Faltan referencias en AssignRenderTexture.");
        }
    }
}
