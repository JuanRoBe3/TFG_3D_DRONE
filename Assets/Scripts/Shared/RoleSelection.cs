using UnityEngine;

/*
   ////NOTES////
 - used in role selection scene
 - role selectd saved on PlayerPrefs so it is accesible during the whole "system run" + 
   corresponding scene loaded depending on the role +
   subscription to the corresponding topics depending on the role chosen: "assignRoleMQTTClient()"
*/ 
public class RoleSelection : MonoBehaviour
{
    // Constants for roles to avoid hardcoded strings
    private const string CommanderRole = "Commander";
    private const string PilotRole = "Pilot";
    private const string RoleKey = "Role"; // Key for storing role in PlayerPrefs

    // Public properties for role identification
    public static bool IsCommander => PlayerPrefs.GetString(RoleKey) == CommanderRole;
    public static bool IsPilot => PlayerPrefs.GetString(RoleKey) == PilotRole;

    void Start()
    {
        MQTTClient.EnsureExists();
        AssetBundleManager.EnsureExists();
    }

    public void SelectCommander()
    {
        // ⚠️ Nota: la IP usada luego dependerá del rol, ver MQTTConfig.GetBrokerIP()
        PlayerPrefs.SetString(RoleKey, CommanderRole);
        PlayerPrefs.Save();
        SceneLoader.LoadCommanderUI(); // Load Commander's main scene
        assignRoleMQTTClient();
    }

    public void SelectPilot()
    {
        // ⚠️ Nota: la IP usada luego dependerá del rol, ver MQTTConfig.GetBrokerIP()
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
