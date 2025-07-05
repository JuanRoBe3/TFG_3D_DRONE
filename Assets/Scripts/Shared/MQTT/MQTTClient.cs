using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;
using UnityEngine;

/// <summary>
/// Cliente MQTT centralizado para Unity. Maneja conexión, reconexión y distribución de mensajes.
/// Ahora con soporte para múltiples handlers por topic.
/// </summary>
public class MQTTClient : MonoBehaviour
{
    public static MQTTClient Instance { get; private set; }

    private IMqttClient mqttClient;
    private IMqttClientOptions mqttOptions;
    private string clientId;

    public event Action<string, string> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnReconnectCompleted; // ✅ NUEVO

    private Dictionary<string, List<Action<string>>> topicHandlers = new();
    private HashSet<string> subscribedTopics = new();
    private readonly Dictionary<string, List<string>> roleTopicMap = MQTTTopicSubscriptions.RoleTopics;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        clientId = "Client-" + Guid.NewGuid();

        await Reconnect(MQTTConfig.GetBrokerIP());
    }

    public async Task Reconnect(string newIP)
    {
        if (mqttClient.IsConnected)
            await mqttClient.DisconnectAsync();

        mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(newIP, MQTTConstants.BrokerPort)
            .WithCredentials(MQTTConstants.Username, MQTTConstants.Password)
            .WithCleanSession(false)
            .Build();

        await ConnectToMQTT();

        // ✅ Notifica a quienes deban re-registrar handlers
        OnReconnectCompleted?.Invoke();
    }

    private async Task ConnectToMQTT()
    {
        try
        {
            var result = await mqttClient.ConnectAsync(mqttOptions);
            Debug.Log($"🔌 MQTT conectado como {clientId}, result: {result?.ResultCode ?? (object)"(sin ResultCode)"}");

            if (mqttClient.IsConnected)
            {
                ResubscribeAllTopics();
                OnConnected?.Invoke();
            }

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string topic = e.ApplicationMessage.Topic.Trim();
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Debug.Log($"📥 [MQTT RX] Topic recibido: «{topic}»");

                if (topicHandlers.TryGetValue(topic, out var handlers))
                {
                    Debug.Log($"✅ {handlers.Count} handler(s) registrados para: «{topic}»");
                    foreach (var handler in handlers)
                    {
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            try { handler.Invoke(payload); }
                            catch (Exception ex)
                            {
                                Debug.LogError($"❌ Error en handler de topic {topic}: {ex.Message}");
                            }
                        });
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ No hay handler registrado para topic: «{topic}»");
                    OnMessageReceived?.Invoke(topic, payload);
                }

                return Task.CompletedTask;
            });

            mqttClient.UseDisconnectedHandler(async _ =>
            {
                Debug.LogWarning("⚠️ MQTT desconectado. Reintentando en 2s...");
                await Task.Delay(2000);
                await Reconnect(MQTTConfig.GetBrokerIP());
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error al conectar MQTT: " + ex.Message);
        }
    }

    public async void OnRoleSelected()
    {
        if (!mqttClient.IsConnected)
        {
            Debug.LogWarning("⚠️ MQTT no conectado para suscribir rol");
            return;
        }

        string role = PlayerPrefs.GetString("Role");
        if (roleTopicMap.TryGetValue(role, out var topics))
        {
            foreach (string topic in topics)
                await SubscribeToTopic(topic);
        }
        else
        {
            Debug.LogWarning("⚠️ No hay topics para rol: " + role);
        }
    }

    public async Task SubscribeToTopic(string topic)
    {
        if (mqttClient.IsConnected)
        {
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Debug.Log("🔔 Subscrito a: " + topic);
        }
        else
        {
            Debug.LogWarning("⏳ Topic pendiente de conexión: " + topic);
        }

        subscribedTopics.Add(topic);
    }

    private async void ResubscribeAllTopics()
    {
        foreach (string topic in subscribedTopics)
        {
            try
            {
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
                Debug.Log("🔁 Re-subscrito a: " + topic);
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Error al re-suscribir: " + topic + " → " + ex.Message);
            }
        }
    }

    public void RegisterHandler(string topic, Action<string> handler)
    {
        string normalized = topic.Trim();

        if (!topicHandlers.ContainsKey(normalized))
            topicHandlers[normalized] = new List<Action<string>>();

        if (!topicHandlers[normalized].Contains(handler))
        {
            topicHandlers[normalized].Add(handler);
            Debug.Log($"📌 [MQTT REGISTER] Añadido handler para topic: «{normalized}»");
        }
    }

    public void UnregisterHandler(string topic, Action<string> handler)
    {
        string normalized = topic.Trim();

        if (topicHandlers.ContainsKey(normalized))
        {
            topicHandlers[normalized].Remove(handler);
            Debug.Log($"❌ [MQTT UNREGISTER] Handler eliminado de: «{normalized}»");

            if (topicHandlers[normalized].Count == 0)
            {
                topicHandlers.Remove(normalized);
                Debug.Log($"🧹 [MQTT UNREGISTER] Topic sin handlers → eliminado: «{normalized}»");
            }
        }
    }

    public IMqttClient GetClient() => mqttClient;

    public static void EnsureExists()
    {
        if (Instance == null)
        {
            GameObject obj = new("MQTTClient");
            obj.AddComponent<MQTTClient>();
        }
    }

    public static bool HasInstance => Instance != null;
}
