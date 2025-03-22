using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class CommanderUI1 : MonoBehaviour
{
    public TMP_Text positionText; // UI element to display drone position
    private string tempMessage = "Texto inicial - Si cambia, TMP funciona ✅";

    private void Start()
    {
        if (positionText == null)
        {
            Debug.LogError("❌ ERROR: positionText NO está asignado en el Inspector.");
            return;
        }

        Debug.Log("✅ positionText está asignado, probando cambio de texto...");
        
        // 🔹 PRUEBA 1: Cambiar el texto directamente en Start()
        positionText.text = "Texto cambiado en Start() ✅";
        positionText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();

        if (MQTTClient.Instance != null)
        {
            Debug.Log("CommanderUI1: Suscribiendo a MQTTClient...");
            MQTTClient.Instance.OnMessageReceived += UpdatePositionText;
        }
        else
        {
            Debug.LogError(LogMessagesConstants.ErrorMQTTClientNotFound);
        }
    }

    private void UpdatePositionText(string topic, string message)
    {
        if (topic == MQTTConstants.DronePositionTopic)
        {
            if (positionText == null)
            {
                Debug.LogError("❌ ERROR: positionText NO está asignado en el Inspector.");
                return;
            }

            Debug.Log($"📍 Actualizando UI con posición: {message}");
            
            tempMessage = LogMessagesConstants.DronePositionPrefix + message;
            StartCoroutine(ApplyTextUpdateCoroutine());
        }
    }

    private IEnumerator ApplyTextUpdateCoroutine()
    {
        yield return null;

        positionText.text = tempMessage;
        positionText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(positionText.transform.parent as RectTransform);
        EventSystem.current.SetSelectedGameObject(null); // activa el sistema de eventos
    }


    private void Update()
    {
        // 🔹 PRUEBA 2: Actualizar el texto en Update() cada frame
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
