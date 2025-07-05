using UnityEngine;

public class PilotReadyBroadcaster : MonoBehaviour
{
    void Start()
    {
        MQTTClient.EnsureExists();
        var client = MQTTClient.Instance.GetClient();

        if (client != null && client.IsConnected)
        {
            new MQTTPublisher(client).PublishMessage(MQTTConstants.PilotReadyForSearchingZone, "true");
            Debug.Log("🎯 PilotReady enviado");
        }
        else
        {
            Debug.LogWarning("⚠️ MQTT no conectado; ready se enviará al reconectar");
            MQTTClient.Instance.OnConnected += () =>
            {
                new MQTTPublisher(client).PublishMessage(MQTTConstants.PilotReadyForSearchingZone, "true");
                Debug.Log("🎯 PilotReady reenviado tras reconexión");
            };
        }
    }
}
