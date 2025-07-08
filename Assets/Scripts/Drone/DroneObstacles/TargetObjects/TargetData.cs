using UnityEngine;

public class TargetData : MonoBehaviour
{
    [Header("Target Info")]
    public string targetId = "UNKNOWN";

    private bool isDiscovered = false;
    public bool IsDiscovered => isDiscovered;

    public void MarkAsDiscoveredLocally()
    {
        if (isDiscovered) return;
        isDiscovered = true;
        Debug.Log($"📍 Target marcado como descubierto localmente: {targetId}");
    }
}
