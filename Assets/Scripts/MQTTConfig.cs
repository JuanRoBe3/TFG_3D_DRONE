using UnityEngine;

public static class MQTTConfig
{
    private static string defaultIP = MQTTConstants.DefaultBrokerIP; // Default IP from constants

    public static string GetBrokerIP()
    {
        return PlayerPrefs.GetString("IPCommander", defaultIP);
    }

    public static void SetBrokerIP(string ip)
    {
        PlayerPrefs.SetString("IPCommander", ip);
        PlayerPrefs.Save();
        Debug.Log(LogMessagesConstants.DebugBrokerIPUpdated + ip);
    }
}