using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DroneMarkerClickHandler : MonoBehaviour
{
    private string droneId;

    public void Configure(string id) => droneId = id;

    void OnMouseDown()
    {
        Debug.Log($"🟢 Click en dron con ID: {droneId}");
    }
}
