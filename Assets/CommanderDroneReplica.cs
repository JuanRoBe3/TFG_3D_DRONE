using UnityEngine;

public class CommanderDroneReplica : MonoBehaviour
{
    [Header("Prefab references")]
    [SerializeField] private Camera fpvCam;                     // Cámara para la vista del piloto
    [SerializeField] private DroneCameraReplicator replicator;  // Script que replica la posición/rotación por MQTT
    [SerializeField] private ClickableDrone clicker;            // Objeto que se puede clicar
    [SerializeField] private Transform visualTransform;         // Objeto visual que rota en Y

    private string droneId;

    // ✅ Usamos DroneData directamente en lugar de pasar un string
    public void Init(DroneData droneData)
    {
        if (droneData == null)
        {
            Debug.LogError("❌ DroneData es null en CommanderDroneReplica");
            return;
        }

        droneId = droneData.droneName;

        // 1. RenderTexture
        var rt = RenderTextureRegistry.GetOrCreate(droneId);
        fpvCam.targetTexture = rt;
        DroneViewPanelManager.Register(droneId, rt);

        // 2. Replicación
        if (replicator != null)
        {
            replicator.SetDroneId(droneId);
            replicator.SetRoot(this.transform);        // Mueve todo el prefab
            replicator.SetVisual(visualTransform);     // Rota solo el visual
            replicator.SetFPVCamera(fpvCam.transform); // Solo la cámara
        }

        // 3. Clickable
        if (clicker == null)
            clicker = GetComponentInChildren<ClickableDrone>();

        if (clicker != null)
            clicker.SetId(droneId);
    }

    public Camera GetCamera()
    {
        return fpvCam;
    }
}
