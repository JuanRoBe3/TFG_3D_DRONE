using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using UnityEngine;

/// <summary>
/// Encapsula la lógica de publicación de mensajes MQTT.
/// Pensado para publicar mensajes no-retained y con QoS 1.
/// </summary>
public class MQTTPublisher
{
    private readonly IMqttClient client;

    public MQTTPublisher(IMqttClient mqttClient)
    {
        client = mqttClient;
    }

    /// <summary>
    /// Publica un mensaje MQTT en el topic indicado (QoS 1, no retained).
    /// </summary>
    /// <param name="topic">Nombre del topic</param>
    /// <param name="payload">Contenido del mensaje en formato string (normalmente JSON)</param>
    public async Task PublishMessage(string topic, string payload)
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogError("❌ No se puede publicar: cliente MQTT nulo o no conectado.");
            return;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false) // 🔒 desactivado para evitar errores con múltiples zonas
            .Build();

        try
        {
            await client.PublishAsync(message);
            Debug.Log($"📤 Publicado MQTT → Topic: {topic} | Payload: {payload}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error al publicar MQTT en topic {topic} → {ex.Message}");
        }
    }
}
