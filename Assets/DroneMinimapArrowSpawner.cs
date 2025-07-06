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

        GameObject arrow = Instantiate(arrowPrefab);
        arrow.name = $"DirectionArrow_{drone.name}";

        var follow = arrow.GetComponent<FollowAndRotate>();
        follow.SetTarget(drone.transform);
    }
}
