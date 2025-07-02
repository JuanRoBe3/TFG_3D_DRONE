using UnityEngine;

public class ClickableDrone : MonoBehaviour
{
    [SerializeField] private string droneId;

    public void SetId(string id) => droneId = id;

    /*
    void OnMouseDown()   // para escritorio
    {
        if (string.IsNullOrEmpty(droneId))
        {
            Debug.LogWarning("⚠️ DroneId vacío en ClickableDrone");
            return;
        }
        DroneViewPanelManager.ShowDrone(droneId);
    } 
    */
    public string GetDroneId() => droneId;          // para log/debug

    public void TriggerSelection()                 // llamado desde el selector
    {
        if (string.IsNullOrEmpty(droneId))
        {
            Debug.LogWarning("ClickableDrone sin droneId asignado");
            return;
        }
        DroneViewPanelManager.ShowDrone(droneId);
    }

}
