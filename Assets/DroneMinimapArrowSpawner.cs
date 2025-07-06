using UnityEngine;

public class DroneMinimapArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;

    private void OnEnable()
    {
        DroneLoader.OnDroneInstantiated += HandleDroneInstantiated;
    }

    private void OnDisable()
    {
        DroneLoader.OnDroneInstantiated -= HandleDroneInstantiated;
    }

    private void HandleDroneInstantiated(GameObject drone)
    {
        if (arrowPrefab == null || drone == null)
        {
            Debug.LogWarning("❌ No se puede instanciar flecha. Prefab o dron es null.");
            return;
        }

        // 1️⃣ Instanciar flecha
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.name = $"DirectionArrow_{drone.name}";

        // 2️⃣ Hacer que siga al dron
        var follow = arrow.GetComponent<FollowAndRotate>();
        follow?.SetTarget(drone.transform);

        // 3️⃣ Conectar visual warning del arrow al detector del dron
        var warningManager = arrow.GetComponent<DroneVisualWarningManager>();
        var detector = drone.GetComponent<ObstacleDetector>();

        if (warningManager != null && detector != null)
        {
            detector.visualWarningManager = warningManager;
            Debug.Log("🔗 VisualWarningManager asignado dinámicamente al ObstacleDetector.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo asignar el VisualWarningManager al detector.");
        }
    }
}
