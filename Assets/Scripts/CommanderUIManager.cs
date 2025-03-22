using UnityEngine;

public class CommanderUIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject statusPanel;
    public GameObject commandsPanel;
    public GameObject logsPanel;

    private void Start()
    {
        ShowMainMenu(); // Start with the main menu active
    }

    public void ShowMainMenu()
    {
        ActivatePanel(mainMenuPanel);
    }

    public void ShowStatus()
    {
        ActivatePanel(statusPanel);
    }

    public void ShowCommands()
    {
        ActivatePanel(commandsPanel);
    }

    public void ShowLogs()
    {
        ActivatePanel(logsPanel);
    }

    private void ActivatePanel(GameObject panelToActivate)
    {
        mainMenuPanel.SetActive(panelToActivate == mainMenuPanel);
        statusPanel.SetActive(panelToActivate == statusPanel);
        commandsPanel.SetActive(panelToActivate == commandsPanel);
        logsPanel.SetActive(panelToActivate == logsPanel);
    }
}
