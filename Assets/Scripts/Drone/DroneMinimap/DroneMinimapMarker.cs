using UnityEngine;

public class DroneMinimapMarker : MonoBehaviour
{
    public string droneTag = "Drone";
    public RectTransform icon;
    public RectTransform viewCone;
    public RectTransform rangeCircle;
    public RectTransform minimapContainer;
    public float mapScale = 1f;

    private Transform droneTransform;

    void Update()
    {
        // Si aún no hemos encontrado el dron, lo buscamos por tag
        if (droneTransform == null)
        {
            GameObject drone = GameObject.FindGameObjectWithTag(droneTag);
            if (drone != null)
                droneTransform = drone.transform;
        }

        if (droneTransform == null)
            return;

        // Posición y rotación del icono
        Vector2 mapPos = new Vector2(droneTransform.position.x, droneTransform.position.z) * mapScale;

        icon.anchoredPosition = mapPos;
        viewCone.anchoredPosition = mapPos;
        rangeCircle.anchoredPosition = mapPos;

        float yaw = droneTransform.eulerAngles.y;
        icon.localEulerAngles = new Vector3(0, 0, -yaw);
        viewCone.localEulerAngles = new Vector3(0, 0, -yaw);
    }
}
