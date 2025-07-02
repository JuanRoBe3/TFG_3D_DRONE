using UnityEngine;

public class ClickableDrone : MonoBehaviour
{
    [SerializeField] private string droneId;

    public void SetId(string id) => droneId = id;

    public string GetDroneId() => droneId;

    public void TriggerSelection()
    {
        if (string.IsNullOrEmpty(droneId))
        {
            Debug.LogWarning("ClickableDrone sin droneId asignado");
            return;
        }

        Debug.Log($"🟢 TriggerSelection llamado para '{droneId}'");

        DroneViewPanelManager.ShowAllViews(droneId);
    }
}
