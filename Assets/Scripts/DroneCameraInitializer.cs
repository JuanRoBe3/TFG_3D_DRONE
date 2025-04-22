using UnityEngine;

public class DroneCameraInitializer : MonoBehaviour
{
    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += AssignCameras;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= AssignCameras;
    }

    private void AssignCameras(GameObject droneInstance)
    {
        Debug.Log("🔍 Drone instanciado, buscando cámaras...");

        Transform firstPersonCam = FindChildRecursive(droneInstance.transform, "PilotCamera");
        Transform topDownCam = FindChildRecursive(droneInstance.transform, "TopDownCamera");

        if (firstPersonCam == null || topDownCam == null)
        {
            Debug.LogError("❌ No se encontraron las cámaras 'PilotCamera' o 'TopDownCamera' dentro del dron.");
            return;
        }

        DroneCameraPublisher publisher = Object.FindFirstObjectByType<DroneCameraPublisher>();
        if (publisher == null)
        {
            Debug.LogError("❌ No se encontró DroneCameraPublisher en la escena.");
            return;
        }

        publisher.SetCameras(firstPersonCam, topDownCam);
        Debug.Log("✅ Cámaras asignadas al DroneCameraPublisher correctamente.");
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
