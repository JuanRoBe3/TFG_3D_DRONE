using UnityEngine;
using UnityEngine.UI;

//Script for adjusting the position of the Canvas in front of the camera in the piloting scene

public class HUDRebinder : MonoBehaviour
{
    [SerializeField] Canvas canvasHUD;     // arrastra tu CanvasHUD aquí

    void Awake()
    {
        // Espera a que el dron aparezca y luego engancha el HUD
        DroneLoader.OnDroneInstantiated += OnDrone;
    }

    void OnDestroy()
    {
        DroneLoader.OnDroneInstantiated -= OnDrone;
    }

    void OnDrone(GameObject drone)
    {
        // Localiza el CenterEyeAnchor del rig dentro del prefab
        var eye = drone.transform
                       .Find("GimbalPivot/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        if (!eye)
        {
            Debug.LogError("CenterEyeAnchor no encontrado en el dron instanciado");
            return;
        }

        // Re-parent y reseteo local
        canvasHUD.transform.SetParent(eye, worldPositionStays: false);
        canvasHUD.transform.localPosition = new Vector3(0, -0.5f, 3f);
        canvasHUD.transform.localRotation = Quaternion.identity;
        canvasHUD.transform.localScale = Vector3.one * 0.002f;

        Debug.Log("✅ CanvasHUD re-parentado al visor del dron");
    }
}
