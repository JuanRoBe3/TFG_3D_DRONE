using System;
using UnityEngine;

//DE MOMENTO NO SIRVE DE UN CAGAO ASÍ QUE TOCARÁ BORRAR

public class MQTTSubscriber
{
    private readonly MQTTClient mqttClient;

    public MQTTSubscriber()
    {
        mqttClient = MQTTClient.Instance;

        if (mqttClient == null)
        {
            Debug.LogError("❌ MQTTSubscriber: MQTTClient.Instance is null.");
            return;
        }

        mqttClient.OnMessageReceived += HandleMessageReceived;
    }

    private void HandleMessageReceived(string topic, string payload)
    {
        Debug.Log($"📨 MQTTSubscriber received - Topic: {topic}, Payload: {payload}");

        // Aquí puedes filtrar por topic y ejecutar acciones según el contenido
        switch (topic)
        {
            case MQTTConstants.DronePositionTopic:
                HandleDronePosition(payload);
                break;

            case MQTTConstants.DroneStatusTopic:
                HandleDroneStatus(payload);
                break;

            default:
                Debug.Log($"⚠️ Unhandled topic: {topic}");
                break;
        }
    }

    private void HandleDronePosition(string payload)
    {
        // Ejemplo: parsear JSON, actualizar UI, etc.
        Debug.Log($"📍 New drone position: {payload}");
    }

    private void HandleDroneStatus(string payload)
    {
        Debug.Log($"📡 Drone status: {payload}");
    }

    public void Unsubscribe()
    {
        if (mqttClient != null)
            mqttClient.OnMessageReceived -= HandleMessageReceived;
    }
}
