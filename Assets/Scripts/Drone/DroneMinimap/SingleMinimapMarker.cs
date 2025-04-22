using UnityEngine;

public class SingleMinimapMarker : MonoBehaviour
{
    public string droneTag = "Drone";                 // Tag del dron
    public RectTransform markerImage;                 // El ícono del dron (círculo + flecha + cono)
    public float mapScale = 1f;                       // (ya no se usa en esta opción, pero lo dejamos por si vuelves a la otra)

    private Transform droneTransform;

    void Update()
    {
        // Buscar el dron si aún no lo tenemos
        if (droneTransform == null)
        {
            GameObject drone = GameObject.FindGameObjectWithTag(droneTag);
            if (drone != null)
                droneTransform = drone.transform;
        }

        if (droneTransform == null || markerImage == null)
            return;

        // 💡 El dron siempre está en el centro del minimapa
        markerImage.anchoredPosition = Vector2.zero;

        /*
        // La rotación del ícono (flecha) debe representar el yaw del dron
        float yaw = droneTransform.eulerAngles.y;
        markerImage.localEulerAngles = new Vector3(0, 0, -yaw); // rotación inversa porque en UI es horaria 
         */

        // No rotamos el marcador si la cámara cenital es fija
        markerImage.localEulerAngles = Vector3.zero;

    }
}
