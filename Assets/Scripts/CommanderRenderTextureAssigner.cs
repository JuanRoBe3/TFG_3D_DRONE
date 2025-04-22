using UnityEngine;

/// <summary>
/// Asigna las RenderTextures del comandante a sus cámaras.
/// Solo usa texturas del RenderTextureRegistry locales (no comparte con el piloto).
/// </summary>
public class CommanderRenderTextureAssigner : MonoBehaviour
{
    [Header("Manual override (opcional)")]
    public Camera commanderFirstPersonCamera;
    public Camera commanderTopDownCamera;

    private void OnEnable()
    {
        AssignTexturesToCameras();
    }

    private void AssignTexturesToCameras()
    {
        if (RenderTextureRegistry.Instance == null)
        {
            Debug.LogError("❌ RenderTextureRegistry no encontrado.");
            return;
        }

        // 🧠 Buscar cámaras por nombre si no están asignadas
        if (commanderFirstPersonCamera == null)
            commanderFirstPersonCamera = GameObject.Find("CommanderFirstPersonCamera")?.GetComponent<Camera>();

        if (commanderTopDownCamera == null)
            commanderTopDownCamera = GameObject.Find("CommanderTopDownCamera")?.GetComponent<Camera>();

        if (commanderFirstPersonCamera == null || commanderTopDownCamera == null)
        {
            Debug.LogError("❌ No se encontraron las cámaras del comandante.");
            return;
        }

        // ✅ Asignar RenderTextures desde el registro
        commanderFirstPersonCamera.targetTexture = RenderTextureRegistry.Instance.commanderFirstPersonTexture;
        commanderTopDownCamera.targetTexture = RenderTextureRegistry.Instance.commanderTopDownTexture;

        Debug.Log("✅ RenderTextures asignadas a cámaras del comandante correctamente.");
    }
}
