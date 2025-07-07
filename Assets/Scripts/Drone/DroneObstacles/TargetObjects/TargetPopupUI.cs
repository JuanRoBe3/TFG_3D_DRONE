using UnityEngine;
using TMPro;

public class TargetPopupUI : MonoBehaviour
{
    public GameObject popupPanel;
    public TextMeshProUGUI targetIdText;
    public TextMeshProUGUI directionText;

    public void ShowTargetInfo(string targetId, string direction)
    {
        popupPanel.SetActive(true);
        targetIdText.text = $"🧍 ID: {targetId}";
        directionText.text = $"🧭 Dir: {direction}";
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }
}
