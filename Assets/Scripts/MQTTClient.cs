using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using UnityEngine;

public class MQTTClient : MonoBehaviour
{
    public static MQTTClient Instance { get; private set; }

    private IMqttClient client;
    private IMqttClientOptions options;
    private string uniqueClientId;

    // Define an event to notify when a message is received
    public event System.Action<string, string> OnMessageReceived;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning(LogMessagesConstants.WarningDuplicateMQTTClient);
            Destroy(gameObject);
            return;
        }
    }

    async void Start()
    {
        var factory = new MqttFactory();
        client = factory.CreateMqttClient();

        // ✅ Generar un Client ID único para evitar desconexiones
        uniqueClientId = "Client-" + System.Guid.NewGuid().ToString();

        options = new MqttClientOptionsBuilder()
            .WithClientId(uniqueClientId)  // 🔹 Ahora cada instancia tendrá un Client ID único
            .WithTcpServer(MQTTConfig.GetBrokerIP(), MQTTConstants.BrokerPort)
            .WithCredentials(MQTTConstants.Username, MQTTConstants.Password)
            .WithCleanSession()
            .Build();

        await ConnectToMQTT();
    }

    private async Task ConnectToMQTT()
    {
        try
        {
            var result = await client.ConnectAsync(options);
            Debug.Log($"🔌 MQTT Client '{uniqueClientId}' connected! Result: {result.ResultCode}");

            // ✅ Manejador de mensajes MQTT
            client.UseApplicationMessageReceivedHandler(e =>
            {
                if (e.ApplicationMessage == null)
                {
                    Debug.LogError("⚠️ Received a null MQTT message!");
                    return;
                }

                Debug.Log($"📩 Message received in Unity - Topic: '{e.ApplicationMessage.Topic}', Message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

                OnMessageReceived?.Invoke(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            });
        }
        catch (Exception ex)
        {
            Debug.LogError(LogMessagesConstants.ErrorMQTTConnectionFailed + ex.Message);
        }
    }

    public async void OnRoleSelected()
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogError("⚠️ Cannot subscribe, MQTT client is not connected.");
            return;
        }

        // ✅ Suscripción basada en el rol seleccionado
        if (RoleSelection.IsCommander)
        {
            await SubscribeToTopic(MQTTConstants.DroneStatusTopic);
            await SubscribeToTopic(MQTTConstants.DronePositionTopic);
        }
        else if (RoleSelection.IsPilot)
        {
            await SubscribeToTopic(MQTTConstants.CommandTopic);
        }
    }

    private async Task SubscribeToTopic(string topic)
    {
        if (client.IsConnected)
        {
            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Debug.Log($"🔔 Subscribed to topic: {topic}");
        }
        else
        {
            Debug.LogError("⚠️ Cannot subscribe, the client is not connected.");
        }
    }

    public async void PublishMessage(string topic, string payload)
    {
        if (client.IsConnected)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await client.PublishAsync(message);
            Debug.Log($"📤 Publishing message to {topic}: {payload}");
        }
        else
        {
            Debug.LogError("⚠️ The message could not be sent, the client is not connected.");
        }
    }
}
