using UnityEngine;

public class PilotReadyListener : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("🚀 [PilotReadyListener] Awake");

        MQTTClient.EnsureExists();
        Debug.Log("✅ [PilotReadyListener] MQTTClient.EnsureExists() ejecutado");

        var client = MQTTClient.Instance.GetClient();
        if (client != null)
        {
            Debug.Log($"🔌 [PilotReadyListener] Cliente MQTT detectado. ¿Conectado?: {client.IsConnected}");

            if (client.IsConnected)
            {
                Debug.Log("✅ [PilotReadyListener] Cliente conectado → Registrando handler");
                Register();
            }
            else
            {
                Debug.Log("⏳ [PilotReadyListener] Cliente aún NO conectado → esperando evento OnConnected");
                MQTTClient.Instance.OnConnected += Register;
            }
        }
        else
        {
            Debug.LogError("❌ [PilotReadyListener] Cliente MQTT NULL. Verifica conexión");
        }
    }

    private void Register()
    {
        Debug.Log("🔁 [PilotReadyListener] Registrando handler para topic PilotReadyForSearchingZone");

        MQTTClient.Instance.RegisterHandler(MQTTConstants.PilotReadyForSearchingZone, OnPilotReady);
        Debug.Log($"🛰️ SUSCRITO a {MQTTConstants.PilotReadyForSearchingZone}");
    }

    private void OnPilotReady(string payload)
    {
        Debug.Log($"📥 [PilotReadyListener] Mensaje recibido en topic {MQTTConstants.PilotReadyForSearchingZone}: payload = {payload}");

        var client = MQTTClient.Instance.GetClient();
        if (client == null || !client.IsConnected)
        {
            Debug.LogError("❌ [PilotReadyListener] Cliente MQTT no disponible o desconectado al intentar reenviar zonas");
            return;
        }

        var zones = SearchZoneRegistry.GetAllZones();
        Debug.Log($"📦 [PilotReadyListener] Reenviando {zones.Count} zonas desde el registro");

        foreach (var z in zones)
        {
            if (z == null)
            {
                Debug.LogWarning("⚠️ [PilotReadyListener] Zona null en el registro. Ignorada.");
                continue;
            }

            var json = JsonUtility.ToJson(z);
            new MQTTPublisher(client).PublishMessage(MQTTConstants.SearchingZone, json);
            Debug.Log($"📤 Zona enviada → ID: {z.id}");
        }

        if (zones.Count == 0)
        {
            Debug.Log("📭 [PilotReadyListener] No hay zonas en el registro para reenviar");
        }
    }


    void OnDestroy()
    {
        Debug.Log("🧹 [PilotReadyListener] OnDestroy → Desuscribiendo handlers");

        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.PilotReadyForSearchingZone, OnPilotReady);
            MQTTClient.Instance.OnConnected -= Register;
        }
    }
}
