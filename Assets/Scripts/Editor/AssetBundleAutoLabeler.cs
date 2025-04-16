using UnityEditor;
using UnityEngine;

public class AssetBundleAutoLabeler
{
    [MenuItem("Tools/Auto-Label DroneData Assets")]
    public static void LabelDroneDataAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:DroneData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter importer = AssetImporter.GetAtPath(path);

            if (importer != null)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                importer.assetBundleName = filename.ToLower(); // o "dronebundle_" + filename
                Debug.Log($"🔖 Asignado bundle '{filename}' a: {path}");
            }
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        Debug.Log("✅ Etiquetado automático completo.");
    }
}
