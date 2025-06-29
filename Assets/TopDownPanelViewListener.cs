using UnityEngine;

public class TopDownViewPanelListener : MonoBehaviour
{
    [SerializeField] private CommanderCameraConfigurator camConfig;

    void OnEnable()
    {
        camConfig.ResetToDefaultView();
        Debug.Log("🔁 Panel TopDown activado. Vista restablecida.");
    }
}
