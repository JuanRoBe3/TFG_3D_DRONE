using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class DroneLoader : MonoBehaviour
{
    public static event Action<GameObject> OnDroneInstantiated;

    void Start()
    {
        AssetBundleManager.EnsureExists();
        LoadDroneFromSelectedInfo();
    }

    private void LoadDroneFromSelectedInfo()
    {
        DroneData selectedDrone = SelectedDroneHolder.GetDrone();
        if (selectedDrone == null)
        {
            Debug.LogError("❌ No hay dron seleccionado.");
            return;
        }

        string fileName = selectedDrone.assetBundleName.EndsWith(".bundle")
            ? selectedDrone.assetBundleName
            : selectedDrone.assetBundleName + ".bundle";

        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput", fileName);
        AssetBundle bundle = AssetBundleManager.Instance.LoadBundle(bundlePath);
        if (bundle == null)
        {
            Debug.LogError($"❌ No se pudo cargar el AssetBundle en: {bundlePath}");
            return;
        }

        GameObject prefab = bundle.LoadAsset<GameObject>(selectedDrone.name);
        if (prefab == null)
        {
            Debug.LogError($"❌ No se encontró el prefab '{selectedDrone.name}' en el bundle.");
            return;
        }

        // 📍 Calcular posición segura usando WorldBounds
        Vector3 spawnPos;

        if (WorldBounds.Value.size == Vector3.zero)
        {
            spawnPos = new Vector3(0f, 10f, 0f);
            Debug.LogWarning("⚠️ WorldBounds no inicializado, usando posición por defecto.");
        }
        else
        {
            Vector3 centerXZ = new Vector3(WorldBounds.Value.center.x, 0f, WorldBounds.Value.center.z);
            float safeY = WorldBounds.Value.max.y + 2.5f;  // margen vertical
            spawnPos = new Vector3(centerXZ.x, safeY, centerXZ.z);
            Debug.Log($"📍 Instanciando dron en posición segura: {spawnPos}");
        }

        // Instanciar dron
        GameObject drone = Instantiate(prefab, spawnPos, Quaternion.identity);
        drone.tag = "Drone";
        Debug.Log("✅ Dron instanciado correctamente desde bundle");

        // 🔔 Invocar evento
        OnDroneInstantiated?.Invoke(drone);
    }
}
