using System.Text;
using MQTTnet;
using MQTTnet.Client;
using UnityEngine;

/// <summary>
/// Encapsula la lógica de publicación de mensajes MQTT.
/// Requiere una instancia de IMqttClient ya conectada.
/// </summary>
public class MQTTPublisher
{
    private readonly IMqttClient client;

    /// <summary>
    /// Constructor del publisher. Requiere un cliente MQTT ya conectado.
    /// </summary>
    /// <param name="mqttClient">Cliente MQTT conectado</param>
    public MQTTPublisher(IMqttClient mqttClient)
    {
        client = mqttClient;
    }

    /// <summary>
    /// Publica un mensaje MQTT en un topic determinado.
    /// </summary>
    /// <param name="topic">Topic al que se enviará el mensaje</param>
    /// <param name="payload">Contenido del mensaje</param>
    public async void PublishMessage(string topic, string payload)
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogError("❌ Cannot publish: MQTT client is null or not connected.");
            return;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await client.PublishAsync(message);
        Debug.Log($"📤 Published to {topic}: {payload}");
    }
}
