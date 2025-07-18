using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
   ////NOTES////
 - used in IP selection scene (pilot)
 - saving IP and conneting to it: "SaveIPAndConnect()"
 - should write as an input the IP of the commander if it the commander is conneted in another PC of the same net +
   for testing in the sae computer should write 127.0.0.1 
*/
public class IPConfigUI : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public Button connectButton;

    void Start()
    {
        // Mostramos la IP guardada del piloto (si existe)
        ipInputField.text = PlayerPrefs.GetString("IPPilot", "");
        connectButton.interactable = !string.IsNullOrEmpty(ipInputField.text);
        ipInputField.onValueChanged.AddListener(delegate { ValidateInput(); });
    }

    private void ValidateInput()
    {
        //Comprueba que no esté vacío
        connectButton.interactable = !string.IsNullOrEmpty(ipInputField.text);
    }

    public async void SaveIPAndConnect()
    {
        string ip = ipInputField.text;

        if (!string.IsNullOrEmpty(ip))
        {
            MQTTConfig.SetPilotIP(ip);                           // 💾 Guardamos la IP del piloto
            MQTTClient.EnsureExists();                           // ⚙️ Aseguramos que el cliente MQTT existe
            await MQTTClient.Instance.Reconnect(ip);            // 🔁 Conectamos usando la IP introducida
            MQTTClient.Instance.OnRoleSelected();               // 📡 Suscribimos a los topics correspondientes
            SceneLoader.LoadPilotDroneSelectionUI();            // 🚀 Avanzamos a la siguiente escena
        }
    }
}
