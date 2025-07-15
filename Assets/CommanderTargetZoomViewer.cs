using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommanderTargetZoomViewer : MonoBehaviour
{
    [Header("UI donde se mostrará el zoom del target")]
    [SerializeField] private RawImage zoomPanel;

    [Header("Texto opcional para mostrar el ID")]
    [SerializeField] private TextMeshProUGUI idText;

    [Header("Cámara de zoom en escena")]
    [SerializeField] private Camera zoomCamera; // 🆕

    private const string RT_KEY = "TargetZoom_RT"; // 🆕 clave única para el RenderTexture

    public static CommanderTargetZoomViewer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowZoomForTarget(string targetId)
    {
        if (zoomPanel == null || zoomCamera == null)
        {
            Debug.LogWarning("❌ zoomPanel o zoomCamera no asignados");
            return;
        }

        // 1️⃣ Obtener la vista (posición + rotación)
        if (!TargetDiscoveryReceiver.Instance.TryGetDiscoveryView(targetId, out var pos, out var rot))
        {
            Debug.LogWarning($"❌ No se encontró vista guardada para el target {targetId}");
            return;
        }

        // 2️⃣ Asignar RT si no está aún
        var rt = RenderTextureRegistry.GetOrCreate(RT_KEY, 1024);
        zoomCamera.targetTexture = rt;
        zoomPanel.texture = rt;

        // 3️⃣ Mover la cámara
        zoomCamera.transform.position = pos;
        zoomCamera.transform.rotation = rot;
        zoomCamera.enabled = true;

        // 4️⃣ Mostrar panel
        zoomPanel.gameObject.SetActive(true);
        if (idText != null) idText.text = $"Target: {targetId}";

        Debug.Log($"🔍 Mostrando zoom para target: {targetId}");
    }

    public void HideZoom()
    {
        if (zoomPanel != null) zoomPanel.gameObject.SetActive(false);
        if (idText != null) idText.text = "";

        if (zoomCamera != null)
        {
            zoomCamera.enabled = false;
            zoomCamera.targetTexture = null;
        }
    }
}
