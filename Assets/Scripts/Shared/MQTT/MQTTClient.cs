using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
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

    /// <summary>
    /// Evento que se dispara cuando se recibe un mensaje MQTT.
    /// Proporciona el topic y el contenido del mensaje.
    /// </summary>
    public event Action<string, string> OnMessageReceived;

    // ✅ Diccionario de topics por rol
    private readonly Dictionary<string, List<string>> roleTopicMap = new()
    {
        { "Commander", new List<string> { MQTTConstants.DroneStatusTopic, MQTTConstants.DronePositionTopic } },
        { "Pilot",     new List<string> { MQTTConstants.CommandTopic } }
        // Si agregas más roles, los añades aquí
    };

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

        uniqueClientId = "Client-" + Guid.NewGuid().ToString();

        options = new MqttClientOptionsBuilder()
            .WithClientId(uniqueClientId)
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

            client.UseApplicationMessageReceivedHandler(e =>
            {
                if (e.ApplicationMessage == null)
                {
                    Debug.LogError("⚠️ Received a null MQTT message!");
                    return;
                }

                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                Debug.Log($"📩 MQTT received - Topic: '{topic}', Payload: {payload}");

                // 🔁 Delegar el procesamiento al exterior mediante evento
                OnMessageReceived?.Invoke(topic, payload);
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

        string role = PlayerPrefs.GetString("Role");

        if (roleTopicMap.TryGetValue(role, out var topics))
        {
            foreach (var topic in topics)
            {
                await SubscribeToTopic(topic);
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ No topics configured for role: {role}");
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

    public IMqttClient GetClient()
    {
        return client;
    }
}
