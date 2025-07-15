using UnityEngine;
using UnityEngine.UI;

public class ZoomedTargetViewPanelManager : MonoBehaviour
{
    [Header("🎯 Panel UI que contiene la vista del target")]
    [SerializeField] private CanvasGroup zoomPanel;

    [Header("📺 RawImage donde se muestra la cámara")]
    [SerializeField] private RawImage zoomImage;

    [Header("🎥 Cámara usada para renderizar la vista del target")]
    [SerializeField] private Camera zoomCamera;

    private static ZoomedTargetViewPanelManager instance;
    public static ZoomedTargetViewPanelManager Instance => instance;

    private RenderTexture zoomRT; // 💡 Guardamos la referencia para usarla varias veces

    void Awake()
    {
        instance = this;

        // ✅ Crear y asignar RenderTexture
        zoomRT = RenderTextureRegistry.GetOrCreate("TargetZoom_RT", 1024);
        zoomCamera.targetTexture = zoomRT;

        // 🛑 Impide que esta cámara se use en VR
        zoomCamera.stereoTargetEye = StereoTargetEyeMask.None;

        Hide(); // Ocultar panel al inicio
    }

    public static void ShowTargetView(string targetId)
    {
        if (instance == null)
        {
            Debug.LogError("❌ ZoomedTargetViewPanelManager no está inicializado");
            return;
        }

        if (!TargetDiscoveryReceiver.Instance.TryGetDiscoveryView(targetId, out var pos, out var rot))
        {
            Debug.LogWarning($"⚠️ No se encontró una vista registrada para el target '{targetId}'");
            return;
        }

        // Posicionar la cámara según la vista guardada
        instance.zoomCamera.transform.SetPositionAndRotation(pos, rot);

        // ✅ Activar la cámara
        instance.zoomCamera.enabled = true;

        // ✅ Asignar RenderTexture al RawImage (CRÍTICO)
        instance.zoomImage.texture = instance.zoomRT;

        // ✅ Mostrar el panel
        instance.zoomPanel.alpha = 1f;
        instance.zoomPanel.interactable = true;
        instance.zoomPanel.blocksRaycasts = true;

        Debug.Log($"🔍 Mostrando vista zoom del target: {targetId}");
    }

    public static void Hide()
    {
        if (instance == null) return;

        // 🔴 Desactivar cámara y panel
        //instance.zoomCamera.enabled = false;
        instance.zoomPanel.alpha = 0f;
        instance.zoomPanel.interactable = false;
        instance.zoomPanel.blocksRaycasts = false;
    }
}
