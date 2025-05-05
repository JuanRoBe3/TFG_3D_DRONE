using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class DroneLoader : MonoBehaviour
{
    public static event Action<GameObject> OnDroneInstantiated;

    void Start()
    {
        AssetBundleManager.EnsureExists(); // 🔒 Asegura que existe
        LoadDroneFromSelectedInfo();
    }

    private void LoadDroneFromSelectedInfo()
    {
        DroneData selectedDrone = SelectedDroneHolder.GetDrone();

        if (selectedDrone == null)
        {
            Debug.LogError("❌ No se ha seleccionado ningún dron.");
            return;
        }

        string bundleName = selectedDrone.assetBundleName;
        string assetName = selectedDrone.name;

        if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("❌ El dron seleccionado no tiene definido bundleName o assetName.");
            return;
        }

        string fileName = bundleName.EndsWith(".bundle") ? bundleName : bundleName + ".bundle";
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput", fileName);
        Debug.Log($"📦 Cargando AssetBundle desde: {bundlePath}");

        AssetBundle bundle = AssetBundleManager.Instance.LoadBundle(bundlePath);
        if (bundle == null)
        {
            Debug.LogError("❌ Error al cargar el AssetBundle.");
            return;
        }

        GameObject dronePrefab = bundle.LoadAsset<GameObject>(assetName);

        if (dronePrefab == null)
        {
            Debug.LogError($"❌ Prefab '{assetName}' no encontrado dentro del AssetBundle.");
            return;
        }

        GameObject droneInstance = Instantiate(dronePrefab);
        droneInstance.tag = "Drone";
        Debug.Log("✅ Dron instanciado desde AssetBundle.");

        // 🔔 Notificar al sistema que el dron ha sido instanciado
        OnDroneInstantiated?.Invoke(droneInstance);

        ObstacleDetector detector = droneInstance.GetComponent<ObstacleDetector>();
        DroneHUDWarningManager hudManager = Object.FindFirstObjectByType<DroneHUDWarningManager>();
        CollisionDistanceUI distanceUI = Object.FindFirstObjectByType<CollisionDistanceUI>();

        if (detector != null)
        {
            detector.hudWarningManager = hudManager;
            detector.distanceUI = distanceUI;
            detector.obstacleLayer = LayerMask.GetMask("Terrain");

            Debug.Log("✅ ObstacleDetector: HUD, UI y capa asignados.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró componente ObstacleDetector en el dron.");
        }

        ActivatePilotCamera(droneInstance);
        AssignPilotCameraToSharedCanvas(droneInstance);

        if (selectedDrone != null)
        {
            Debug.Log($"🔋 Battery: {selectedDrone.maxBattery}%");
            Debug.Log($"📡 Range: {selectedDrone.maxRange}m");
            Debug.Log($"💾 Storage: {selectedDrone.storageCapacityMB}MB");
            Debug.Log($"⏱️ Duration: {selectedDrone.estimatedFlightDurationMinutes} min");
        }

        TargetPopupUI popupUI = Object.FindFirstObjectByType<TargetPopupUI>();
        TargetDetector targetDetector = droneInstance.GetComponentInChildren<TargetDetector>();

        if (popupUI != null && targetDetector != null)
        {
            targetDetector.popupUI = popupUI;
            Debug.Log("✅ TargetDetector conectado correctamente al TargetPopupUI.");
        }
        else
        {
            if (popupUI == null)
                Debug.LogWarning("⚠️ No se encontró TargetPopupUI en la escena.");
            if (targetDetector == null)
                Debug.LogWarning("⚠️ No se encontró TargetDetector en el dron instanciado.");
        }
    }

    private void ActivatePilotCamera(GameObject droneInstance)
    {
        if (Camera.main != null)
        {
            Camera.main.enabled = false;
            AudioListener listener = Camera.main.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }

        Transform cameraTransform = FindChildByName(droneInstance.transform, "PilotCamera");

        if (cameraTransform != null)
        {
            Camera droneCamera = cameraTransform.GetComponent<Camera>();
            AudioListener droneListener = cameraTransform.GetComponent<AudioListener>();

            if (droneCamera != null) droneCamera.enabled = true;
            if (droneListener != null) droneListener.enabled = true;

            Debug.Log("🎥 Cámara del dron activada.");
        }
        else
        {
            Debug.Log("⚠️ El dron no contiene un hijo llamado 'PilotCamera'.");
        }
    }

    private void AssignPilotCameraToSharedCanvas(GameObject droneInstance)
    {
        Canvas canvas = GameObject.Find("CanvasPilotUI")?.GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("🚫 No se encontró el Canvas 'CanvasPilotUI' en la escena.");
            return;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = null;

        Debug.Log("✅ CanvasPilotUI forzado a Overlay (sin cámara).");
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
