using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CommanderDroneReplica : MonoBehaviour
{
    [Header("Prefab references")]
    [SerializeField] private Camera fpvCam;
    [SerializeField] private DroneCameraReplicator replicator;
    [SerializeField] private ClickableDrone clicker;

    private string droneId;

    public void Init(string id)
    {
        droneId = id;
        if (fpvCam == null) { Debug.LogError("❌ FPVCamera missing"); return; }

        // 1. RenderTexture centralizada
        var rt = RenderTextureRegistry.GetOrCreate(droneId);   // ← ya usa RTFactory
        fpvCam.targetTexture = rt;
        DroneViewPanelManager.Register(droneId, rt);

        // 2. Replicador (solo para drones “vivos”)
        if (replicator != null)
        {
            replicator.SetCamera(fpvCam.transform);
            replicator.SetDroneId(droneId);
        }

        // 3. Clickable
        if (clicker == null) clicker = GetComponentInChildren<ClickableDrone>();
        if (clicker != null) clicker.SetId(droneId);
    }

    public Camera GetCamera()
    {
        return fpvCam;
    }
}
