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
        MainMini,    // minimap_mainUI_droneView
        MainBigger,  // bigmap_droneView
        MinimapTop   // minimap_topDownCameraUI_droneView
    }

    [Header("Paneles y RawImages")]
    [SerializeField] private CanvasGroup miniGroup;
    [SerializeField] private RawImage miniImage;

    [SerializeField] private CanvasGroup bigGroup;
    [SerializeField] private RawImage bigImage;

    [SerializeField] private CanvasGroup minimapGroup;
    [SerializeField] private RawImage minimapImage;

    private static Dictionary<string, RenderTexture> lookup = new();
    private static DroneViewPanelManager instance;

    void Awake()
    {
        instance = this;
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

        instance.HideAll();

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

    private void Show(CanvasGroup group, RawImage image, RenderTexture rt)
    {
        image.texture = rt;
        group.alpha = 1;
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
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}
