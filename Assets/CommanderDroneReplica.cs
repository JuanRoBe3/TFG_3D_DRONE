using UnityEngine;

public class CommanderDroneReplica : MonoBehaviour
{
    [Header("Prefab references")]
    [SerializeField] private Camera fpvCam;                     // Cámara que renderiza la vista del piloto
    [SerializeField] private DroneCameraReplicator replicator;  // Script que replica posición/rotación
    [SerializeField] private ClickableDrone clicker;            // Permite hacer click sobre el icono del dron
    [SerializeField] private Transform visualTransform;         // ⬅️ Objeto que rota solo en Y (esfera)

    private string droneId;

    public void Init(string id)
    {
        droneId = id;

        if (fpvCam == null)
        {
            Debug.LogError("❌ FPVCamera missing");
            return;
        }

        // 1. RenderTexture desde el registry
        var rt = RenderTextureRegistry.GetOrCreate(droneId);
        fpvCam.targetTexture = rt;
        DroneViewPanelManager.Register(droneId, rt);

        // 2. Replicación de datos (posición y rotaciones)
        if (replicator != null)
        {
            replicator.SetDroneId(droneId);
            replicator.SetRoot(this.transform);                 // 🔸 mueve el objeto entero
            replicator.SetVisual(visualTransform);              // 🔸 solo yaw para la esfera u otro visual
            replicator.SetFPVCamera(fpvCam.transform);          // 🔸 rotación completa
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
