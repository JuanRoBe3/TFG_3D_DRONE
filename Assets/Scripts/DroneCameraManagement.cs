using UnityEngine;
using UnityEngine.UI;

public class DroneCameraManagement : MonoBehaviour
{
    [SerializeField] private RawImage topDownRawImage;
    [SerializeField] private RenderTexture topDownRenderTexture;

    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += HandleDroneInstantiated;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= HandleDroneInstantiated;
    }

    private void HandleDroneInstantiated(GameObject drone)
    {
        // Buscar la cámara cenital dentro del dron instanciado
        Transform topDownCameraTransform = FindChildByName(drone.transform, "TopDownCamera");
        if (topDownCameraTransform == null)
        {
            Debug.LogError("🚨 No se encontró 'TopDownCamera' en el dron instanciado.");
            return;
        }

        Camera topDownCamera = topDownCameraTransform.GetComponent<Camera>();
        if (topDownCamera == null)
        {
            Debug.LogError("🚨 El objeto 'TopDownCamera' no tiene un componente Camera.");
            return;
        }

        topDownCamera.targetTexture = topDownRenderTexture;
        Debug.Log($"📷 Cámara cenital conectada a RenderTexture: {topDownRenderTexture.name}");

        if (topDownRawImage != null)
        {
            topDownRawImage.texture = topDownRenderTexture;
            Debug.Log($"🖼️ RawImage del minimapa conectada al RenderTexture.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó la RawImage para mostrar el minimapa.");
        }
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindChildByName(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
