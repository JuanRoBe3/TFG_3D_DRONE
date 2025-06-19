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
            Debug.LogError("❌ No seleccionado");
            return;
        }

        string fileName = selectedDrone.assetBundleName.EndsWith(".bundle")
            ? selectedDrone.assetBundleName
            : selectedDrone.assetBundleName + ".bundle";

        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput", fileName);
        AssetBundle bundle = AssetBundleManager.Instance.LoadBundle(bundlePath);
        if (bundle == null)
        {
            Debug.LogError("❌ Error al cargar bundle");
            return;
        }

        GameObject prefab = bundle.LoadAsset<GameObject>(selectedDrone.name);
        if (prefab == null)
        {
            Debug.LogError("❌ Prefab no encontrado");
            return;
        }

        // Instanciar dron
        GameObject drone = Instantiate(prefab);
        drone.tag = "Drone";
        Debug.Log("✅ Dron instanciado");

        // Ya NO conectamos la cámara del visor al dron

        // 🔔 Invocar evento de creación
        OnDroneInstantiated?.Invoke(drone);
    }
}
