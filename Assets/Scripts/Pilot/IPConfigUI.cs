using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IPConfigUI : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public Button connectButton;

    void Start()
    {
        // Load the last saved broker IP, if available
        ipInputField.text = MQTTConfig.GetBrokerIP();

        // Ensure the button is clickable only when an IP is entered
        connectButton.interactable = !string.IsNullOrEmpty(ipInputField.text);

        // Add listener for text changes to enable/disable the button dynamically
        ipInputField.onValueChanged.AddListener(delegate { ValidateInput(); });
    }

    private void ValidateInput()
    {
        connectButton.interactable = !string.IsNullOrEmpty(ipInputField.text);
    }

    public void SaveIPAndConnect()
    {
        string ip = ipInputField.text;
        if (!string.IsNullOrEmpty(ip))
        {
            MQTTConfig.SetBrokerIP(ip); // Save the IP using MQTTConfig
            SceneLoader.LoadPilotUI(); // Move to the pilot's main scene
        }
    }
}
