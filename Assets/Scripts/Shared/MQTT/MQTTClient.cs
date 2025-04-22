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
    /// Evento antiguo para compatibilidad (aún puedes usarlo)
    /// </summary>
    public event Action<string, string> OnMessageReceived;

    // 🔁 Nuevo: Diccionario de handlers específicos por topic
    private Dictionary<string, Action<string>> topicHandlers = new Dictionary<string, Action<string>>();

    // 🔁 Ya existente: tópicos por rol
    private readonly Dictionary<string, List<string>> roleTopicMap = MQTTTopicSubscriptions.RoleTopics;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠ Duplicate MQTTClient detected and destroyed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

                // ✅ Modular: redirige según el topic
                if (topicHandlers.TryGetValue(topic, out Action<string> handler))
                {
                    handler.Invoke(payload);
                }
                else
                {
                    // Solo para debug: aún puedes mantener el evento global si quieres
                    OnMessageReceived?.Invoke(topic, payload);
                }
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

    public void RegisterHandler(string topic, Action<string> handler)
    {
        if (!topicHandlers.ContainsKey(topic))
        {
            topicHandlers.Add(topic, handler);
            Debug.Log($"✅ Handler registrado para el topic: {topic}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Ya hay un handler registrado para el topic: {topic}");
        }
    }

    public void UnregisterHandler(string topic)
    {
        if (topicHandlers.ContainsKey(topic))
        {
            topicHandlers.Remove(topic);
            Debug.Log($"❌ Handler eliminado para el topic: {topic}");
        }
    }

    public IMqttClient GetClient()
    {
        return client;
    }

    public static void EnsureExists()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("MQTTClient");
            obj.AddComponent<MQTTClient>();
        }
    }
}
