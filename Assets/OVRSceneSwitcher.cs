using UnityEngine;

public class OVRSceneSwitcher : MonoBehaviour
{
    private void Update()
    {
        // Detectar botón X del controlador izquierdo (OVRInput.Button.One + LTouch)
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Debug.Log("🎮 Botón Y izquierdo pulsado → Cargando escena de selección de dron");
            SceneLoader.LoadPilotDroneSelectionUI();
        }
    }
}
