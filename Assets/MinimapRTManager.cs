using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class MinimapRTManager : MonoBehaviour
{
    [SerializeField] RawImage mapRaw;
    [SerializeField] int size = 1024;

    void Awake()
    {
        if (!RoleSelection.IsPilot) { enabled = false; return; }

        Camera cam = GetComponent<Camera>();
        if (!mapRaw)
        {
            Debug.LogError("MinimapRTInitializer: asigna el RawImage del minimapa");
            return;
        }

        // Crear RT solo si falta
        if (cam.targetTexture == null || mapRaw.texture == null)
        {
            RenderTexture rt = new RenderTexture(size, size, 16);
            rt.name = "MinimapRT";
            rt.Create();

            cam.targetTexture = rt;
            mapRaw.texture = rt;
        }
    }
}
