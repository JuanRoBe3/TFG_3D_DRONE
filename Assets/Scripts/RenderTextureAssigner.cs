using UnityEngine;

/// <summary>
/// Asigna la RenderTexture SOLO a la cámara cenital del dron.
/// NO se asigna nada a la cámara de primera persona, para que sea la cámara principal de pantalla.
/// </summary>
public class RenderTextureAssigner : MonoBehaviour
{
    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += AssignRenderTextures;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= AssignRenderTextures;
    }

    private void AssignRenderTextures(GameObject drone)
    {
        if (RenderTextureRegistry.Instance == null)
        {
            Debug.LogError("❌ RenderTextureRegistry no encontrado en la escena.");
            return;
        }

        // 🔍 Buscamos las cámaras dentro del dron instanciado
        Transform fpCam = FindChildRecursive(drone.transform, "PilotCamera");
        Transform tdCam = FindChildRecursive(drone.transform, "TopDownCamera");

        if (fpCam == null)
            Debug.LogWarning("⚠️ No se encontró PilotCamera dentro del dron instanciado.");
        if (tdCam == null)
            Debug.LogWarning("⚠️ No se encontró TopDownCamera dentro del dron instanciado.");

        if (fpCam == null && tdCam == null)
        {
            Debug.LogError("❌ Ninguna de las cámaras fue encontrada.");
            return;
        }

        // ✅ Asignamos la RenderTexture a la cámara cenital (minimapa)
        if (tdCam != null)
        {
            Camera tdCameraComp = tdCam.GetComponent<Camera>();
            if (tdCameraComp != null)
            {
                tdCameraComp.targetTexture = RenderTextureRegistry.Instance.topDownTexture;
                Debug.Log("✅ RenderTexture asignada a TopDownCamera.");
            }
            else
            {
                Debug.LogWarning("⚠️ TopDownCamera encontrada pero no tiene componente Camera.");
            }
        }

        // ❌ OPCIONAL: descomenta esto si algún día quieres mostrar la PilotCamera en un RawImage
        
        if (fpCam != null)
        {
            Camera fpCameraComp = fpCam.GetComponent<Camera>();
            if (fpCameraComp != null)
            {
                fpCameraComp.targetTexture = RenderTextureRegistry.Instance.firstPersonTexture;
                Debug.Log("✅ RenderTexture asignada a PilotCamera.");
            }
        }
        
    }

    /// <summary>
    /// Busca un hijo recursivamente por nombre exacto.
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = FindChildRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
