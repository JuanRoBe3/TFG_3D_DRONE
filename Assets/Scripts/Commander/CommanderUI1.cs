using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CommanderUI1 : MonoBehaviour
{
    public TMP_Text positionText;       // Texto principal que muestra la última posición
    public TMP_Text positionLogText;    // Texto de log con las últimas 10 posiciones

    private string tempMessage = "";
    private Queue<string> positionLog = new Queue<string>(10); // FIFO de 10 elementos

    private MQTTPublisher publisher;

    private void Start()
    {
        MQTTClient.EnsureExists();

        if (MQTTClient.Instance != null)
        {
            publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
            MQTTClient.Instance.RegisterHandler(MQTTConstants.DroneCameraTopic, HandlePositionPayload);
        }
        else
        {
            Debug.LogError(LogMessagesConstants.ErrorMQTTClientNotFound);
        }
    }

    private void OnDestroy()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(MQTTConstants.DroneCameraTopic);
            Debug.Log("📴 CommanderUI1 DESACTIVADO y handler limpiado");
        }
    }

    private void Update()
    {
        if (positionText != null)
        {
            positionText.text = tempMessage;
        }

        if (positionLogText != null)
        {
            positionLogText.text = string.Join("\n", positionLog);
        }
    }

    private void HandlePositionPayload(string message)
    {
        tempMessage = LogMessagesConstants.DronePositionPrefix + message;

        if (positionLog.Count >= 10)
        {
            positionLog.Dequeue();
        }

        positionLog.Enqueue(tempMessage);

        Debug.Log($"📍 Posición recibida: {tempMessage}");
    }

    /*
    public void SendCommand()
    {
        string command = "{\"action\": \"move\", \"direction\": \"forward\"}";

        if (publisher != null)
        {
            publisher.PublishMessage(MQTTConstants.CommandTopic, command);
            Debug.Log(LogMessagesConstants.DebugMQTTPublished + command);
        }
        else
        {
            Debug.LogError(LogMessagesConstants.ErrorMessageNotSent);
        }
    } 
    */
}
