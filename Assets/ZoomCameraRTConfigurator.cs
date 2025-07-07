using UnityEngine;
using UnityEngine.UI;

public class ZoomCameraRTConfigurator : MonoBehaviour
{
    [Header("Cámara de zoom del visor")]
    [SerializeField] private Camera zoomCamera;

    [Header("RawImage donde se mostrará la vista")]
    [SerializeField] private RawImage zoomRawImage;

    private const string RT_KEY = "ZoomView_RT";

    void Start()
    {
        if (zoomCamera == null)
        {
            Debug.LogError("❌ No se asignó la cámara de zoom.");
            return;
        }

        // Obtiene o crea la RenderTexture centralizada
        var rt = RenderTextureRegistry.GetOrCreate(RT_KEY, 512);

        zoomCamera.targetTexture = rt;

        if (zoomRawImage != null)
        {
            zoomRawImage.texture = rt;
            zoomRawImage.gameObject.SetActive(false); // Oculta por defecto
        }

        zoomCamera.gameObject.SetActive(false); // También oculta por defecto
        Debug.Log("🔍 ZoomCamera configurada con RenderTexture.");
    }
}
