using UnityEngine;

public class DroneTopDownFollower : MonoBehaviour
{
    private Transform droneTransform;
    public float height = 50f;

    void Start()
    {
        DroneLoader.OnDroneInstantiated += HandleDroneInstantiated;
    }

    void OnDestroy()
    {
        DroneLoader.OnDroneInstantiated -= HandleDroneInstantiated;
    }

    private void HandleDroneInstantiated(GameObject drone)
    {
        droneTransform = drone.transform;
        transform.SetParent(null); // Desvincular rotación
    }

    void LateUpdate()
    {
        if (droneTransform == null) return;

        Vector3 flatPos = new Vector3(droneTransform.position.x, 0f, droneTransform.position.z);
        transform.position = flatPos + Vector3.up * height;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
