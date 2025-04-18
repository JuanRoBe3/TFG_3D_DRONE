using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CommanderUI1 : MonoBehaviour
{
    public TMP_Text positionText;       // Texto principal que muestra la última posición
    public TMP_Text positionLogText;    // Texto de log con las últimas 10 posiciones

    private string tempMessage = "";
    private Queue<string> positionLog = new Queue<string>(10); // FIFO de 10 elementos

    private MQTTPublisher publisher; // ✅ NUEVO

    private void Start()
    {
        MQTTClient.EnsureExists(); // 🔒 Asegura que existe el singleton ANTES de usarlo

        if (MQTTClient.Instance != null)
        {
            publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
            MQTTClient.Instance.OnMessageReceived += UpdatePositionText;
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
            MQTTClient.Instance.OnMessageReceived -= UpdatePositionText;
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

    private void UpdatePositionText(string topic, string message)
    {
        if (topic == MQTTConstants.DronePositionTopic)
        {
            tempMessage = LogMessagesConstants.DronePositionPrefix + message;

            // Añadir al log
            if (positionLog.Count >= 10)
            {
                positionLog.Dequeue(); // Eliminar el más antiguo
            }

            positionLog.Enqueue(tempMessage); // Añadir nuevo al final

            Debug.Log($"📍 Posición recibida: {tempMessage}");
        }
    }

    /*
    public void SendCommand()
    {
        string command = "{\"action\": \"move\", \"direction\": \"forward\"}";

        if (publisher != null)
        {
            publisher.PublishMessage(MQTTConstants.CommandTopic, command); // ✅ CAMBIADO
            Debug.Log(LogMessagesConstants.DebugMQTTPublished + command);
        }
        else
        {
            Debug.LogError(LogMessagesConstants.ErrorMessageNotSent);
        }
    } 
     */
}
