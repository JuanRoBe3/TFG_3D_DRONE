using UnityEngine;

public class DroneCameraManager : MonoBehaviour
{
    public static void ActivatePilotCamera(GameObject droneInstance)
    {
        // Desactiva la cámara principal de la escena
        if (Camera.main != null)
        {
            Camera.main.enabled = false;
            AudioListener listener = Camera.main.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }

        // Busca y activa la cámara del dron
        Transform cameraTransform = droneInstance.transform.Find("PilotCamera");
        if (cameraTransform != null)
        {
            Camera droneCamera = cameraTransform.GetComponent<Camera>();
            AudioListener droneListener = cameraTransform.GetComponent<AudioListener>();

            if (droneCamera != null) droneCamera.enabled = true;
            if (droneListener != null) droneListener.enabled = true;
        }
        else
        {
            Debug.LogWarning("PilotCamera not found in drone prefab.");
        }
    }
}
