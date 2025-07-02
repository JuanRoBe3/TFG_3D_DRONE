using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Gestiona los 3 paneles donde se muestra la vista del dron:
/// - Mini en UI principal
/// - Versión ampliada
/// - Minimap de vista top-down
/// </summary>
public class DroneViewPanelManager : MonoBehaviour
{
    public enum DisplayTarget
    {
        MainMini,
        MainBigger,
        MinimapTop
    }

    [Header("Paneles y RawImages")]
    [SerializeField] private CanvasGroup miniGroup;
    [SerializeField] private RawImage miniImage;

    [Header("Mensaje sin dron seleccionado")]
    [SerializeField] private CanvasGroup noDroneSelectedPanel;

    [SerializeField] private CanvasGroup bigGroup;
    [SerializeField] private RawImage bigImage;

    [SerializeField] private CanvasGroup minimapGroup;
    [SerializeField] private RawImage minimapImage;

    private static Dictionary<string, RenderTexture> lookup = new();
    private static DroneViewPanelManager instance;

    private static string lastDroneId = null;
    private static DisplayTarget? currentDisplay = null;

    void Awake()
    {
        instance = this;
        ShowNoDroneSelectedMessage();
        HideAll();
    }

    public static void Register(string droneId, RenderTexture rt)
    {
        lookup[droneId] = rt;
    }

    public static void ShowDrone(string droneId, DisplayTarget target)
    {
        if (!lookup.TryGetValue(droneId, out var rt))
        {
            Debug.LogWarning($"❌ No se encontró RenderTexture para '{droneId}'");
            return;
        }

        lastDroneId = droneId;
        currentDisplay = target;
        instance.HideAll();
        instance.HideNoDroneSelectedMessage();  // ✅ OCULTA el mensaje

        switch (target)
        {
            case DisplayTarget.MainMini:
                instance.Show(instance.miniGroup, instance.miniImage, rt);
                break;
            case DisplayTarget.MainBigger:
                instance.Show(instance.bigGroup, instance.bigImage, rt);
                break;
            case DisplayTarget.MinimapTop:
                instance.Show(instance.minimapGroup, instance.minimapImage, rt);
                break;
        }
    }

    public static void ShowLastSelected(DisplayTarget target)
    {
        if (string.IsNullOrEmpty(lastDroneId))
        {
            Debug.LogWarning("⚠️ No hay dron seleccionado previamente");
            return;
        }

        ShowDrone(lastDroneId, target);
    }

    public static void ShowAllViews(string droneId)
    {
        if (!lookup.TryGetValue(droneId, out var rt))
        {
            Debug.LogWarning($"❌ No se encontró RenderTexture para '{droneId}'");
            return;
        }

        lastDroneId = droneId;
        currentDisplay = null;
        instance.HideNoDroneSelectedMessage();  // ✅ OCULTA el mensaje

        instance.Show(instance.miniGroup, instance.miniImage, rt);
        instance.Show(instance.bigGroup, instance.bigImage, rt);
        instance.Show(instance.minimapGroup, instance.minimapImage, rt);
    }

    public static void CloseCurrent()
    {
        if (currentDisplay == null) return;

        switch (currentDisplay)
        {
            case DisplayTarget.MainMini:
                instance.Hide(instance.miniGroup);
                break;
            case DisplayTarget.MainBigger:
                instance.Hide(instance.bigGroup);
                break;
            case DisplayTarget.MinimapTop:
                instance.Hide(instance.minimapGroup);
                break;
        }

        currentDisplay = null;
    }

    private void Show(CanvasGroup group, RawImage image, RenderTexture rt)
    {
        image.texture = rt;
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void HideAll()
    {
        Hide(miniGroup);
        Hide(bigGroup);
        Hide(minimapGroup);
    }

    private void Hide(CanvasGroup group)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private void ShowNoDroneSelectedMessage()
    {
        noDroneSelectedPanel.alpha = 1;
        noDroneSelectedPanel.interactable = false;
        noDroneSelectedPanel.blocksRaycasts = false;
    }

    private void HideNoDroneSelectedMessage()
    {
        noDroneSelectedPanel.alpha = 0;
        noDroneSelectedPanel.interactable = false;
        noDroneSelectedPanel.blocksRaycasts = false;
    }
}
