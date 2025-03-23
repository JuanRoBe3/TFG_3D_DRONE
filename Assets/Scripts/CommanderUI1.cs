using UnityEngine;
using TMPro;

public class CommanderUI1 : MonoBehaviour
{
    public TMP_Text positionText; // Referencia al objeto de texto en la UI
    private string tempMessage = "";

    private void Start()
    {
        if (positionText == null)
        {
            Debug.LogError("❌ ERROR: positionText NO está asignado en el Inspector.");
            return;
        }

        positionText.text = "Esperando posición del dron...";

        if (MQTTClient.Instance != null)
        {
            Debug.Log("✅ Suscribiéndose a MQTTClient...");
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

    private void UpdatePositionText(string topic, string message)
    {
        if (topic == MQTTConstants.DronePositionTopic)
        {
            tempMessage = LogMessagesConstants.DronePositionPrefix + message;
            Debug.Log($"📍 Posición actualizada: {tempMessage}");
        }
    }

    private void Update()
    {
        positionText.text = tempMessage;
    }

    public void SendCommand()
    {
        string command = "{\"action\": \"move\", \"direction\": \"forward\"}";

        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.PublishMessage(MQTTConstants.CommandTopic, command);
            Debug.Log(LogMessagesConstants.DebugMQTTPublished + command);
        }
        else
        {
            Debug.LogError(LogMessagesConstants.ErrorMessageNotSent);
        }
    }
}
