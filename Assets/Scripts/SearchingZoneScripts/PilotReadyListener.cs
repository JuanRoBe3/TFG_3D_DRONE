using UnityEngine;

public class PilotReadyListener : MonoBehaviour
{
    void Awake()
    {
        MQTTClient.EnsureExists();

        var client = MQTTClient.Instance.GetClient();
        if (client != null && client.IsConnected)
        {
            Register();
        }
        else
        {
            MQTTClient.Instance.OnConnected += Register;
        }
    }

    private void Register()
    {
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PilotReadyForSearchingZone, OnPilotReady);
        Debug.Log("✅ Suscrito a PilotReadyForSearchingZone");
    }

    private void OnPilotReady(string _)
    {
        var client = MQTTClient.Instance.GetClient();
        foreach (var zone in SearchZoneRegistry.Instance.GetAll())
        {
            var json = JsonUtility.ToJson(zone);
            new MQTTPublisher(client).PublishMessage(MQTTConstants.SearchingZone, json);
        }
        Debug.Log($"📤 Reenviadas {SearchZoneRegistry.Instance.GetAll().Count} zonas al piloto");
    }
}
