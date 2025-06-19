using UnityEngine;

public class DroneDetectionAutoBinder : MonoBehaviour
{
    [SerializeField] private DroneHUDWarningManager warningManager;
    [SerializeField] private CollisionDistanceUI distanceUI;
    [SerializeField] private TargetPopupUI popupUI;

    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += OnDroneCreated;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= OnDroneCreated;
    }

    private void OnDroneCreated(GameObject drone)
    {
        if (drone == null) return;

        // Asignar ObstacleDetector
        var obstacleDetector = drone.GetComponentInChildren<ObstacleDetector>();
        if (obstacleDetector != null)
        {
            obstacleDetector.hudWarningManager = warningManager;
            obstacleDetector.distanceUI = distanceUI;
            Debug.Log("✅ ObstacleDetector enlazado correctamente.");
        }
        else
        {
            Debug.LogWarning("⚠️ ObstacleDetector no encontrado en el dron.");
        }

        // Asignar TargetDetector
        var targetDetector = drone.GetComponentInChildren<TargetDetector>();
        if (targetDetector != null)
        {
            targetDetector.popupUI = popupUI;
            Debug.Log("✅ TargetDetector enlazado correctamente.");
        }
        else
        {
            Debug.LogWarning("⚠️ TargetDetector no encontrado en el dron.");
        }
    }
}
