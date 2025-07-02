using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class MinimapRTManager : MonoBehaviour
{
    [SerializeField] private RawImage mapRaw;

    void Awake()
    {
        if (!RoleSelection.IsPilot) { enabled = false; return; }

        Camera cam = GetComponent<Camera>();
        if (!mapRaw)
        {
            Debug.LogError("MinimapRTManager: asigna el RawImage del minimapa");
            return;
        }

        // Crear RT solo si falta
        if (cam.targetTexture == null || mapRaw.texture == null)
        {
            RenderTexture rt = RTFactory.New();  // Usa los valores por defecto: 1024x1024
            cam.targetTexture = rt;
            mapRaw.texture = rt;
        }
    }
}
