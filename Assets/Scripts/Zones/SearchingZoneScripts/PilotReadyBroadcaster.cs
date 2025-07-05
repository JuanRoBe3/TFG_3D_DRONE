using UnityEngine;
using System.Collections;

public class PilotReadyBroadcaster : MonoBehaviour
{
    private bool zonesReceived = false;
    private Coroutine loop;   // ← guardamos la corrutina para poder pararla

    void Start()
    {
        MQTTClient.EnsureExists();
        loop = StartCoroutine(WaitAndPublishReady());
    }

    IEnumerator WaitAndPublishReady()
    {
        while (!zonesReceived)
        {
            var client = MQTTClient.Instance.GetClient();
            if (client != null && client.IsConnected)
            {
                new MQTTPublisher(client).PublishMessage(
                    MQTTConstants.PilotReadyForSearchingZone, "true");
                Debug.Log("📡 PilotReadyForSearchingZone reenviado");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void NotifyZonesReceived()
    {
        zonesReceived = true;
        if (loop != null) StopCoroutine(loop);   // ← detiene el envío
    }

    void OnDestroy()
    {
        if (loop != null) StopCoroutine(loop);   // ← por si la escena se descarga antes
    }
}
