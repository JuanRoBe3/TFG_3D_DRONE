using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneViewPanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RawImage droneRawImage;
    [SerializeField] private CanvasGroup panelGroup;

    private static Dictionary<string, RenderTexture> lookup = new();
    private static DroneViewPanelManager instance;

    void Awake()
    {
        instance = this;
        Hide();  // Ocultar el panel al empezar
    }

    public static void Register(string droneId, RenderTexture rt)
    {
        lookup[droneId] = rt;
    }

    public static void ShowDrone(string droneId)
    {
        if (!lookup.TryGetValue(droneId, out var rt))
        {
            Debug.LogWarning($"❌ No se encontró RenderTexture para {droneId}");
            return;
        }

        instance.droneRawImage.texture = rt;
        instance.Show();
    }

    public void Hide()
    {
        panelGroup.alpha = 0;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
    }

    private void Show()
    {
        panelGroup.alpha = 1;
        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
    }
}
