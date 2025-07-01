using UnityEngine;

public class DroneCameraInitializer : MonoBehaviour
{
    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += AssignCamera;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= AssignCamera;
    }

    private void AssignCamera(GameObject droneInstance)
    {
        Debug.Log("🔍 Drone instanciado, buscando cámara del piloto...");

        Transform pilotCam = FindChildRecursive(droneInstance.transform, "PilotCamera");

        if (pilotCam == null)
        {
            Debug.LogError("❌ No se encontró la cámara 'PilotCamera' dentro del dron.");
            return;
        }

        DroneCameraPublisher publisher = Object.FindFirstObjectByType<DroneCameraPublisher>();
        if (publisher == null)
        {
            Debug.LogError("❌ No se encontró DroneCameraPublisher en la escena.");
            return;
        }

        publisher.SetCamera(pilotCam);
        Debug.Log("✅ Cámara del dron asignada correctamente al publisher.");
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
