using UnityEngine;
using System.IO;

public class DroneLoader : MonoBehaviour
{
    void Start()
    {
        LoadDroneFromSelectedInfo();
    }

    private void LoadDroneFromSelectedInfo()
    {
        DroneModelInfo selectedDrone = SelectedDroneHolder.GetDrone();

        if (selectedDrone == null)
        {
            Debug.LogError("❌ No se ha seleccionado ningún dron.");
            return;
        }

        string bundleName = selectedDrone.bundleName;
        string assetName = selectedDrone.assetName;

        if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("❌ El dron seleccionado no tiene definido bundleName o assetName.");
            return;
        }

        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput", bundleName);
        Debug.Log($"📦 Cargando AssetBundle desde: {bundlePath}");

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"❌ AssetBundle no encontrado en la ruta: {bundlePath}");
            return;
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

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
        Debug.Log("✅ Dron instanciado desde AssetBundle.");

        DroneData data = droneInstance.GetComponent<DroneData>();

        if (data != null)
        {
            Debug.Log($"🔋 Battery: {data.batteryLevel}%");
            Debug.Log($"📡 Range: {data.maxRange}m");
            Debug.Log($"💾 Storage: {data.storageAvailableMB}MB");
            Debug.Log($"⏱️ Duration: {data.flightDurationSeconds} sec");
        }
        else
        {
            Debug.LogWarning("⚠️ El dron instanciado no contiene componente DroneData.");
        }

        // Descargar el bundle (mantener los assets en memoria)
        bundle.Unload(false);
    }
}
