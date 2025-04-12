using UnityEngine;
using TMPro;

public class CollisionDistanceUI : MonoBehaviour
{
    public GameObject leftTextObject;   // El GameObject que contiene el texto izquierdo
    public GameObject rightTextObject;  // El GameObject que contiene el texto derecho

    public TextMeshProUGUI leftDistanceText;
    public TextMeshProUGUI rightDistanceText;

    public void UpdateLeftDistance(float distance)
    {
        leftTextObject.SetActive(true);
        leftDistanceText.text = $"⬅️ {distance:F1} m";
    }

    public void UpdateRightDistance(float distance)
    {
        rightTextObject.SetActive(true);
        rightDistanceText.text = $"➡️ {distance:F1} m";
    }

    public void ClearLeftDistance()
    {
        leftTextObject.SetActive(false);
    }

    public void ClearRightDistance()
    {
        rightTextObject.SetActive(false);
    }
}
