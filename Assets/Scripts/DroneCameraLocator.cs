using UnityEngine;

public class DroneCameraLocator : MonoBehaviour
{
    public Transform firstPersonCameraTransform { get; private set; }
    public Transform topDownCameraTransform { get; private set; }

    private void Awake()
    {
        firstPersonCameraTransform = FindChildRecursive(transform, "PilotCamera");
        topDownCameraTransform = FindChildRecursive(transform, "TopDownCamera");

        if (firstPersonCameraTransform == null || topDownCameraTransform == null)
        {
            Debug.LogError("❌ No se encontraron las cámaras en el prefab del dron.");
        }
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
