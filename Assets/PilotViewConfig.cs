using UnityEngine;

public static class PilotViewConfig
{
    private const string PrefKey = "PilotViewMode";

    public static PilotViewMode SelectedMode { get; private set; } = PilotViewMode.HUDScreen;

    public static void SetMode(PilotViewMode mode)
    {
        SelectedMode = mode;
        PlayerPrefs.SetInt(PrefKey, (int)mode);
        PlayerPrefs.Save(); // 💾 Guarda en disco
        Debug.Log($"🎮 Modo de vista guardado: {mode}");
    }

    public static void Load()
    {
        SelectedMode = (PilotViewMode)PlayerPrefs.GetInt(PrefKey, (int)PilotViewMode.HUDScreen);
        Debug.Log($"📥 Modo de vista cargado: {SelectedMode}");
    }
}
