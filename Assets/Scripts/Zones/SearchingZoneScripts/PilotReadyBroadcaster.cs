using UnityEngine;
using System.Collections;

public class PilotReadyBroadcaster : MonoBehaviour
{
    private Coroutine loop;

    void Start()
    {
        Debug.Log("🚀 [PilotReadyBroadcaster] Start → Inicializando");

        MQTTClient.EnsureExists();

        var client = MQTTClient.Instance.GetClient();
        if (client != null && client.IsConnected)
        {
            Debug.Log("✅ [PilotReadyBroadcaster] Cliente ya conectado → empieza bucle de envío");
            loop = StartCoroutine(SendReadyLoop());
        }
        else
        {
            Debug.Log("⏳ [PilotReadyBroadcaster] Cliente aún no conectado → espera evento OnConnected");
            MQTTClient.Instance.OnConnected += () =>
            {
                Debug.Log("🔌 [PilotReadyBroadcaster] Cliente conectado → empieza bucle de envío");
                loop = StartCoroutine(SendReadyLoop());
            };
        }
    }

    IEnumerator SendReadyLoop()
    {
        while (true)
        {
            var client = MQTTClient.Instance.GetClient();
            if (client != null && client.IsConnected)
            {
                new MQTTPublisher(client)
                    .PublishMessage(MQTTConstants.PilotReadyForSearchingZone, "ready");

                Debug.Log("📡 [PilotReadyBroadcaster] Mensaje 'ready' reenviado");
            }
            else
            {
                Debug.LogWarning("⚠️ [PilotReadyBroadcaster] Cliente MQTT no conectado. Reintentando...");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // Podrás llamarlo en el futuro si el comandante confirma que recibió el "ready"
    public void Stop()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            Debug.Log("🛑 [PilotReadyBroadcaster] Bucle detenido → Comandante ha confirmado recepción");
        }
    }

    void OnDestroy()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            Debug.Log("🧹 [PilotReadyBroadcaster] OnDestroy → Corrutina detenida");
        }
    }
}
