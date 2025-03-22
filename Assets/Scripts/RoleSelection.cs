using UnityEngine;

public class RoleSelection : MonoBehaviour
{
    // Constants for roles to avoid hardcoded strings
    private const string CommanderRole = "Commander";
    private const string PilotRole = "Pilot";
    private const string RoleKey = "Role"; // Key for storing role in PlayerPrefs

    // Public properties for role identification
    public static bool IsCommander => PlayerPrefs.GetString(RoleKey) == CommanderRole;
    public static bool IsPilot => PlayerPrefs.GetString(RoleKey) == PilotRole;

    public void SelectCommander()
    {
        PlayerPrefs.SetString(RoleKey, CommanderRole);
        PlayerPrefs.Save();
        SceneLoader.LoadCommanderUI(); // Load Commander's main scene
        assignRoleMQTTClient();
    }

    public void SelectPilot()
    {
        PlayerPrefs.SetString(RoleKey, PilotRole);
        PlayerPrefs.Save();
        SceneLoader.LoadPilotIPInput(); // Load screen where Pilot enters the broker IP
        assignRoleMQTTClient();
    }

    public void assignRoleMQTTClient()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.OnRoleSelected(); //  Se suscribe después de elegir rol
        }
    }

}
