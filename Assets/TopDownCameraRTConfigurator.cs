using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Asigna una RenderTexture a la cámara top-down del comandante usando RTRegistry.
/// </summary>
public class TopDownCameraRTConfigurator : MonoBehaviour
{
    [Header("Cámara top-down del comandante")]
    [SerializeField] private Camera topDownCamera;

    [Header("Vista pequeña del minimapa")]
    [SerializeField] private RawImage minimapRawImage;

    [Header("Vista ampliada (clic)")]
    [SerializeField] private RawImage bigmapRawImage;

    private const string RT_KEY = "CommanderTopDown_RT";

    void Start()
    {
        if (topDownCamera == null)
        {
            Debug.LogError("❌ No se asignó la cámara top-down.");
            return;
        }

        // Usa tu sistema centralizado para obtener la RT
        var rt = RenderTextureRegistry.GetOrCreate(RT_KEY, 1024);

        // Asigna a la cámara
        topDownCamera.targetTexture = rt;

        // Asigna a los paneles de UI
        if (minimapRawImage != null) minimapRawImage.texture = rt;
        if (bigmapRawImage != null) bigmapRawImage.texture = rt;

        Debug.Log("🧭 TopDownCamera configurada con RenderTexture.");
    }
}
