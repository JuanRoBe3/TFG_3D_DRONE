using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CommanderDroneManager : MonoBehaviour
{
    public static CommanderDroneManager Instance { get; private set; }

    private List<DroneData> availableDrones = new List<DroneData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllDroneDataFromAssetBundles();
    }

    public List<DroneData> GetAvailableDrones()
    {
        return availableDrones;
    }

    public DroneData GetDroneByName(string droneName)
    {
        return availableDrones.Find(d => d.droneName == droneName);
    }

    private void LoadAllDroneDataFromAssetBundles()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput");
        if (!Directory.Exists(path))
        {
            Debug.LogWarning("❌ No se encontró la carpeta de AssetBundles.");
            return;
        }

        string[] bundleFiles = Directory.GetFiles(path);
        foreach (string bundlePath in bundleFiles)
        {
            if (!bundlePath.EndsWith(".bundle")) continue;

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Debug.LogWarning($"⚠️ Error al cargar el bundle: {bundlePath}");
                continue;
            }

            DroneData[] droneAssets = bundle.LoadAllAssets<DroneData>();
            foreach (DroneData drone in droneAssets)
            {
                availableDrones.Add(drone);
                Debug.Log($"✅ Dron añadido: {drone.droneName} (bundle: {Path.GetFileName(bundlePath)})");
            }

            bundle.Unload(false);
        }
    }
}
