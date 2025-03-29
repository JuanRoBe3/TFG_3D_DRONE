using UnityEngine;
using System.IO;

public class DroneLoader : MonoBehaviour
{
    public string bundleName = "drone2bundle";  // Nombre del archivo bundle (sin extensión)
    public string assetName = "Drone2";         // Nombre del prefab dentro del bundle

    void Start()
    {
        LoadDroneFromAssetBundle();
    }

    private void LoadDroneFromAssetBundle()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput", bundleName);
        Debug.Log("OLA K ASE: " + bundlePath);

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"❌ AssetBundle no encontrado: {bundlePath}");
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
            Debug.LogError($"❌ Prefab '{assetName}' no encontrado en el bundle.");
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

        // Puedes descargar el bundle si no vas a usar más cosas
        bundle.Unload(false);
    }
}
