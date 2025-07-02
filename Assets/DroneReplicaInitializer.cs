using UnityEngine;

// 🚁 Este script se usa para los DUMMY que ya están en la escena
public class DroneReplicaInitializer : MonoBehaviour
{
    [SerializeField] private DroneData droneData;   // 🔹 Asignar en Inspector
    [SerializeField] private Camera fpvCam;         // 🔹 Cámara FPV dentro del prefab

    void Awake()
    {
        if (fpvCam == null)
            fpvCam = GetComponentInChildren<Camera>();

        if (droneData == null)
        {
            Debug.LogError("❌ No se asignó DroneData en Dummy");
            return;
        }

        string droneId = droneData.droneName;

        // 1. Registrar RenderTexture
        var rt = RenderTextureRegistry.GetOrCreate(droneId);
        fpvCam.targetTexture = rt;
        DroneViewPanelManager.Register(droneId, rt);

        // 2. Registrar clic en minimapa (si aplica)
        var clicker = GetComponent<ClickableDrone>();
        if (clicker != null)
            clicker.SetId(droneId);
    }
}
