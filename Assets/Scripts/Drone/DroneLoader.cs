using UnityEngine;
using System.IO;
using System;

public class DroneLoader : MonoBehaviour
{
    public static event Action<GameObject> OnDroneInstantiated;

    void Start()
    {
        AssetBundleManager.EnsureExists();

        // Registra en memoria los drones que tengas definidos en escena/inspector
        DroneRegistry.RegisterAll(DroneSelectionManager.Instance.availableDrones);

        // Carga el dron que el Comandante dejó guardado en PlayerPrefs
        LoadDroneFromExecutingTask();
    }

    //──────────────────────────────────────────────────────────
    //  Carga el dron y prepara la publicación de su cámara
    //──────────────────────────────────────────────────────────
    private void LoadDroneFromExecutingTask()
    {
        // 1️⃣ Recuperar el dron elegido
        string droneName = PlayerPrefs.GetString("SelectedDroneId", null);
        if (string.IsNullOrEmpty(droneName))
        {
            Debug.LogError("❌ PlayerPrefs no contiene 'SelectedDroneId'.");
            return;
        }

        // 2️⃣ Obtener DroneData
        DroneData selectedDrone = DroneRegistry.Get(droneName);
        if (selectedDrone == null)
        {
            Debug.LogError($"❌ DroneRegistry no conoce '{droneName}'.");
            return;
        }

        // 3️⃣ Cargar AssetBundle
        string fileName = selectedDrone.assetBundleName.EndsWith(".bundle")
                          ? selectedDrone.assetBundleName
                          : selectedDrone.assetBundleName + ".bundle";
        string bundlePath = Path.Combine(Application.streamingAssetsPath,
                                         "AssetBundlesOutput", fileName);

        AssetBundle bundle = AssetBundleManager.Instance.LoadBundle(bundlePath);
        if (bundle == null)
        {
            Debug.LogError($"❌ No se pudo cargar el AssetBundle en: {bundlePath}");
            return;
        }

        // 4️⃣ Obtener prefab
        GameObject prefab = bundle.LoadAsset<GameObject>(selectedDrone.name);
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab '{selectedDrone.name}' no encontrado en el bundle.");
            return;
        }

        // 5️⃣ Calcular posición de aparición
        Vector3 spawnPos;
        if (WorldBounds.Value.size == Vector3.zero)
        {
            spawnPos = new Vector3(0f, 10f, 0f);
            Debug.LogWarning("⚠️ WorldBounds no inicializado, usando posición por defecto.");
        }
        else
        {
            Vector3 centerXZ = new Vector3(WorldBounds.Value.center.x, 0f, WorldBounds.Value.center.z);
            float safeY = WorldBounds.Value.max.y + 2.5f;
            spawnPos = new Vector3(centerXZ.x, safeY, centerXZ.z);
            Debug.Log($"📍 Instanciando dron en posición segura: {spawnPos}");
        }

        // 6️⃣ Instanciar el dron
        GameObject drone = Instantiate(prefab, spawnPos, Quaternion.identity);
        drone.tag = "Drone";
        Debug.Log($"✅ Dron '{droneName}' instanciado correctamente");

        // 7️⃣ Localizar la cámara interna del dron
        Transform pilotCam = FindChildRecursive(drone.transform, "PilotCamera");
        if (pilotCam == null)
        {
            Debug.LogError("❌ No se encontró 'PilotCamera' en el prefab.");
            return;
        }

        // 8️⃣ Encontrar el único DroneCameraPublisher colocado en la escena
        DroneCameraPublisher pub = FindAnyObjectByType<DroneCameraPublisher>();
#if UNITY_2021
        // si tu versión es <2022 usa:
        // DroneCameraPublisher pub = GameObject.FindObjectOfType<DroneCameraPublisher>();
#endif
        if (pub == null)
        {
            Debug.LogError("❌ No hay DroneCameraPublisher en la escena.");
            return;
        }

        // 9️⃣ Inicializar el publisher con ID, cámara y MQTT
        pub.Initialize(MQTTClient.Instance.GetClient(), droneName, pilotCam);

        // 🔔 Avisar a quien escuche
        OnDroneInstantiated?.Invoke(drone);

        // 🔟 Limpiar PlayerPrefs
        PlayerPrefs.DeleteKey("SelectedTaskId");
        PlayerPrefs.DeleteKey("SelectedDroneId");
    }

    //──────────────────────────────────────────────────────────
    //  Utilidad recursiva para buscar un hijo por nombre
    //──────────────────────────────────────────────────────────
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
