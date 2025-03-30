using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Other Scenes
    public static void LoadRoleSelection()
    {
        SceneManager.LoadScene(SceneConstants.RoleSelection);
    }

    // Commander Scenes
    public static void LoadCommanderUI()
    {
        SceneManager.LoadScene(SceneConstants.CommanderUI1);
    }

    // Pilot Scenes
    public static void LoadPilotIPInput()
    {
        SceneManager.LoadScene(SceneConstants.PilotIPInput);
    }

    public static void LoadPilotUI()
    {
        SceneManager.LoadScene(SceneConstants.PilotUI1);
    }

    public static void LoadPilotDroneSelectionUI()
    {
        SceneManager.LoadScene(SceneConstants.PilotDroneSelectionUI);
    }
}
