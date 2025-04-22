using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    private Transform droneTransform;
    public float height = 50f; // Altura deseada sobre el dron

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

        // 🔄 Desvincular para que no herede rotación
        transform.SetParent(null);
        Debug.Log("📸 Cámara cenital desvinculada del dron para mantener rotación fija.");
    }

    void LateUpdate()
    {
        if (droneTransform == null) return;

        // 🧠 Solo seguimos la posición XZ, ignorando la rotación del dron en pitch/roll
        Vector3 flatPosition = new Vector3(
            droneTransform.position.x,
            0f,
            droneTransform.position.z
        );

        transform.position = flatPosition + Vector3.up * height;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Totalmente cenital
    }
}
