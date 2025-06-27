using UnityEngine;
using UnityEngine.UI;

public class ViewModeManager : MonoBehaviour
{
    public Canvas mainCanvas;

    public RawImage droneViewFullScreen;
    public RawImage topDownViewFullScreen;

    public void ShowDroneViewOnly()
    {
        mainCanvas.enabled = false;
        droneViewFullScreen.gameObject.SetActive(true);
        topDownViewFullScreen.gameObject.SetActive(false);
    }

    public void ShowTopDownViewOnly()
    {
        mainCanvas.enabled = false;
        droneViewFullScreen.gameObject.SetActive(false);
        topDownViewFullScreen.gameObject.SetActive(true);
    }

    public void RestoreUI()
    {
        mainCanvas.enabled = true;
        droneViewFullScreen.gameObject.SetActive(false);
        topDownViewFullScreen.gameObject.SetActive(false);
    }
}
