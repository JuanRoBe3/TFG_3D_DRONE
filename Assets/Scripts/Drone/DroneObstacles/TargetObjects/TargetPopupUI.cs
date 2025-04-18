using UnityEngine;
using TMPro;

public class TargetPopupUI : MonoBehaviour
{
    public GameObject popupPanel;
    public TextMeshProUGUI targetIdText;
    public TextMeshProUGUI directionText;
    public float popupDuration = 2f;

    private float timer = 0f;

    void Update()
    {
        if (popupPanel.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer >= popupDuration)
            {
                popupPanel.SetActive(false);
            }
        }
    }

    public void ShowTargetInfo(string targetId, string direction)
    {
        popupPanel.SetActive(true);
        targetIdText.text = $"🧍 ID: {targetId}";
        directionText.text = $"🧭 Dir: {direction}";
        timer = 0f;
    }
}
