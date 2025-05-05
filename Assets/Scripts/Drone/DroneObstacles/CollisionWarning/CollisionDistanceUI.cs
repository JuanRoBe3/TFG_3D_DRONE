using UnityEngine;
using TMPro;

public class CollisionDistanceUI : MonoBehaviour
{
    // GameObjects que contienen los textos de advertencia
    public GameObject leftTextObject;
    public GameObject rightTextObject;
    public GameObject backTextObject;

    // Referencias a los componentes TextMeshProUGUI de cada aviso
    public TextMeshProUGUI leftDistanceText;
    public TextMeshProUGUI rightDistanceText;
    public TextMeshProUGUI backDistanceText;

    // Actualizar distancia izquierda
    public void UpdateLeftDistance(float distance)
    {
        leftTextObject.SetActive(true);
        leftDistanceText.text = $"⬅️ {distance:F1} m";
    }

    // Actualizar distancia derecha
    public void UpdateRightDistance(float distance)
    {
        rightTextObject.SetActive(true);
        rightDistanceText.text = $"➡️ {distance:F1} m";
    }

    // Actualizar distancia trasera
    public void UpdateBackDistance(float distance)
    {
        backTextObject.SetActive(true);
        backDistanceText.text = $"⬅️🔙 {distance:F1} m";
    }

    // Limpiar aviso izquierdo
    public void ClearLeftDistance()
    {
        leftTextObject.SetActive(false);
    }

    // Limpiar aviso derecho
    public void ClearRightDistance()
    {
        rightTextObject.SetActive(false);
    }

    // Limpiar aviso trasero
    public void ClearBackDistance()
    {
        backTextObject.SetActive(false);
    }
}
