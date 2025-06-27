using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Active Panels")]
    [SerializeField] private GameObject panelMap;
    [SerializeField] private GameObject panelTopDownView;
    [SerializeField] private GameObject panelDroneView;

    private void HideAllPanels()
    {
        if (panelMap != null) panelMap.SetActive(false);
        if (panelTopDownView != null) panelTopDownView.SetActive(false);
        if (panelDroneView != null) panelDroneView.SetActive(false);
    }

    public void ShowMap()
    {
        HideAllPanels();
        if (panelMap != null) panelMap.SetActive(true);
    }

    public void ShowTopDownView()
    {
        HideAllPanels();
        if (panelTopDownView != null) panelTopDownView.SetActive(true);
    }

    public void ShowDroneView()
    {
        HideAllPanels();
        if (panelDroneView != null) panelDroneView.SetActive(true);
    }
}
