using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
    public static AssetBundleManager Instance { get; private set; }

    private Dictionary<string, AssetBundle> loadedBundles = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AssetBundle LoadBundle(string fullPath)
    {
        string bundleName = Path.GetFileName(fullPath);

        if (loadedBundles.ContainsKey(bundleName))
        {
            Debug.Log($"📦 AssetBundle '{bundleName}' ya estaba cargado.");
            return loadedBundles[bundleName];
        }

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"⚠️ El archivo no existe: {fullPath}");
            return null;
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(fullPath);

        if (bundle == null)
        {
            Debug.LogWarning($"⚠️ Fallo al cargar bundle: {bundleName}");
            return null;
        }

        loadedBundles[bundleName] = bundle;
        Debug.Log($"✅ AssetBundle cargado: {bundleName}");

        return bundle;
    }

    public bool IsBundleLoaded(string bundleName)
    {
        return loadedBundles.ContainsKey(bundleName);
    }

    public void UnloadBundle(string bundleName, bool unloadAllLoadedObjects = false)
    {
        if (loadedBundles.TryGetValue(bundleName, out AssetBundle bundle))
        {
            bundle.Unload(unloadAllLoadedObjects);
            loadedBundles.Remove(bundleName);
            Debug.Log($"🧹 Bundle descargado: {bundleName}");
        }
    }

    public void UnloadAllBundles(bool unloadAllLoadedObjects = false)
    {
        foreach (var kvp in loadedBundles)
        {
            kvp.Value.Unload(unloadAllLoadedObjects);
            Debug.Log($"🧹 Descargando: {kvp.Key}");
        }

        loadedBundles.Clear();
    }

    public IEnumerable<string> GetLoadedBundleNames()
    {
        return loadedBundles.Keys;
    }

    public static void EnsureExists()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("AssetBundleManager");
            obj.AddComponent<AssetBundleManager>();
        }
    }


}
