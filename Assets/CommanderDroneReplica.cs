using UnityEngine;

public class CommanderDroneReplica : MonoBehaviour
{
    [Header("Componentes asignados desde el prefab")]
    [SerializeField] private Camera fpvCam;                    // arrastra FPVCamera
    [SerializeField] private DroneCameraReplicator replicator; // arrastra replicator

    private string droneId;

    public void Init(string id)
    {
        Debug.Log($"🛠️ Init() llamado en {gameObject.name} con ID: {id}");

        droneId = id;

        if (fpvCam == null)
        {
            Debug.LogError("❌ FPVCamera no está asignada en el prefab.");
        }
        else
        {
            Debug.Log("✅ FPVCamera asignada correctamente.");
        }

        if (replicator == null)
        {
            Debug.LogError("❌ DroneCameraReplicator no está asignado en el prefab.");
        }
        else
        {
            Debug.Log("✅ DroneCameraReplicator asignado correctamente.");

            // Conectar replicador
            replicator.SetCamera(fpvCam.transform);
            replicator.SetDroneId(droneId);
        }

        var handler = GetComponentInChildren<DroneMarkerClickHandler>();
        if (handler == null)
        {
            Debug.LogWarning("⚠️ No se encontró DroneMarkerClickHandler en hijos.");
        }
        else
        {
            handler.Configure(droneId);
            Debug.Log("✅ DroneMarkerClickHandler configurado.");
        }
    }

    public Camera GetCamera() => fpvCam;   // lo usa el manager
}
