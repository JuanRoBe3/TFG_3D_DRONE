using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DroneSelectionDropdownManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown droneDropdown;

    private Dictionary<string, string> nameToBundleMap = new Dictionary<string, string>();

    void Start()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, "AssetBundlesOutput");
        Debug.Log("📂 Escaneando: " + fullPath);

        if (!Directory.Exists(fullPath))
        {
            Debug.LogError("❌ Carpeta no encontrada: " + fullPath);
            return;
        }

        List<string> friendlyNames = new List<string>();
        string[] bundleFiles = Directory.GetFiles(fullPath, "*.bundle");

        foreach (string filePath in bundleFiles)
        {
            string bundleName = Path.GetFileName(filePath);

            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null)
            {
                Debug.LogWarning($"⚠️ Fallo al cargar bundle: {bundleName}");
                continue;
            }

            DroneData droneData = bundle.LoadAllAssets<DroneData>()[0];

            if (droneData != null)
            {
                string friendlyName = droneData.droneName;
                friendlyNames.Add(friendlyName);
                nameToBundleMap[friendlyName] = filePath;

                Debug.Log($"✅ Dron añadido: {friendlyName} (bundle: {bundleName})");
            }
            else
            {
                Debug.LogWarning($"❌ No se encontró DroneData en {bundleName}");
            }

            bundle.Unload(false);
        }

        if (friendlyNames.Count == 0)
        {
            Debug.LogWarning("⚠️ No se encontraron drones válidos.");
        }

        PopulateDropdown(friendlyNames);
    }

    void PopulateDropdown(List<string> droneNames)
    {
        if (droneDropdown == null)
        {
            Debug.LogError("❌ Dropdown no asignado.");
            return;
        }

        droneDropdown.ClearOptions();
        droneDropdown.AddOptions(droneNames);
        droneDropdown.RefreshShownValue();

        Debug.Log("✅ Dropdown rellenado con " + droneNames.Count + " opciones.");
    }

    public string GetBundlePathFromSelection()
    {
        string selectedName = droneDropdown.options[droneDropdown.value].text;
        return nameToBundleMap.ContainsKey(selectedName) ? nameToBundleMap[selectedName] : null;
    }
}
