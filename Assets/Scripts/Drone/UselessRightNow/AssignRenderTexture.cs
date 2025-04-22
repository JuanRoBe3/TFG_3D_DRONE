using UnityEngine;

public class AssignRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null && renderTexture != null)
        {
            cam.targetTexture = renderTexture;
            Debug.Log("✅ RenderTexture asignado correctamente.");
        }
        else
        {
            Debug.LogError("❌ Faltan referencias en AssignRenderTexture.");
        }
    }

}
