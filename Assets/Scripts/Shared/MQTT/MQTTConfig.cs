using UnityEngine;

public static class MQTTConfig
{
    private const string pilotIPKey = "IPPilot";
    private const string roleKey = "Role";

    public static string GetBrokerIP()
    {
        string ip;

        string role = PlayerPrefs.GetString(roleKey, "");

        if (role == "Commander")
        {
            ip = MQTTConstants.DefaultBrokerIP;
            Debug.Log("📡 IP for Commander: " + ip);
        }
        else
        {
            ip = PlayerPrefs.GetString(pilotIPKey, "");
            Debug.Log("📡 IP for Pilot: " + ip);
        }

        return ip;
    }

    public static void SetPilotIP(string ip)
    {
        PlayerPrefs.SetString(pilotIPKey, ip);
        PlayerPrefs.Save();
        Debug.Log("💾 IP del piloto guardada: " + ip);
    }
}
