using UnityEngine;

public class PilotViewManager : MonoBehaviour
{
    [Header("HUD Canvas y Subpaneles")]
    [SerializeField] private GameObject screenCameraView;   // ⬅️ solo contiene el RawImage
    [SerializeField] private Camera centerEyeCamera;        // Cámara del visor (CenterEyeAnchor)

    private Camera pilotCamera;                             // Cámara dentro del prefab del dron

    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += SetupViewMode;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= SetupViewMode;
    }

    private void SetupViewMode(GameObject drone)
    {
        pilotCamera = drone.transform.Find("PilotCamera")?.GetComponent<Camera>();

        if (pilotCamera == null)
        {
            Debug.LogError("❌ No se encontró PilotCamera en el dron instanciado.");
            return;
        }

        switch (PilotViewConfig.SelectedMode)
        {
            case PilotViewMode.HUDScreen:
                ApplyHUDScreenMode();
                break;

            case PilotViewMode.FirstPerson:
                ApplyFirstPersonMode();
                break;
        }
    }

    private void ApplyHUDScreenMode()
    {
        // ✅ Activa la pantalla flotante (solo el RawImage)
        if (screenCameraView != null) screenCameraView.SetActive(true);

        if (pilotCamera != null)
        {
            pilotCamera.targetTexture = RenderTextureRegistry.Instance.firstPersonTexture;
            pilotCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }

        if (centerEyeCamera != null)
        {
            centerEyeCamera.enabled = true;
        }

        Debug.Log("✅ Modo HUD activado (pantalla flotante).");
    }

    private void ApplyFirstPersonMode()
    {
        // ❌ Oculta solo la pantalla flotante, pero deja el resto del canvas activo
        if (screenCameraView != null) screenCameraView.SetActive(false);

        if (pilotCamera != null)
        {
            pilotCamera.targetTexture = null;
            pilotCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        }

        if (centerEyeCamera != null)
        {
            centerEyeCamera.enabled = false;
        }

        Debug.Log("✅ Modo First Person activado.");
    }
}
