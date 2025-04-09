using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public string droneTag = "Drone"; // Asegúrate de que el prefab tenga este tag
    public Vector3 offset = new Vector3(0, 50, 0); // Distancia encima del dron

    private Transform droneTransform;

    void Update()
    {
        if (droneTransform == null)
        {
            GameObject droneObj = GameObject.FindGameObjectWithTag(droneTag);
            if (droneObj != null)
            {
                droneTransform = droneObj.transform;
            }
        }
        else
        {
            transform.position = droneTransform.position + offset;
            transform.rotation = Quaternion.Euler(90, 0, 0); // Mira hacia abajo (cenital)
        }
    }
}
