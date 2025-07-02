using UnityEngine;

public class PilotViewManager : MonoBehaviour
{
    [SerializeField] GameObject screenCameraView;
    [SerializeField] Camera centerEyeCamera;

    Camera pilotCamera;

    void OnEnable() { DroneLoader.OnDroneInstantiated += Setup; }
    void OnDisable() { DroneLoader.OnDroneInstantiated -= Setup; }

    void Setup(GameObject drone)
    {
        pilotCamera = drone.transform.Find("PilotCamera")?.GetComponent<Camera>();
        if (!pilotCamera) { Debug.LogError("❌ PilotCamera no encontrada"); return; }

        if (PilotViewConfig.SelectedMode == PilotViewMode.HUDScreen) HUD();
        else FP();
    }

    void HUD()
    {
        if (screenCameraView) screenCameraView.SetActive(true);
        pilotCamera.stereoTargetEye = StereoTargetEyeMask.None;
        if (centerEyeCamera) centerEyeCamera.enabled = true;
    }

    void FP()
    {
        if (screenCameraView) screenCameraView.SetActive(false);
        pilotCamera.targetTexture = null;
        pilotCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        if (centerEyeCamera) centerEyeCamera.enabled = false;
    }
}
