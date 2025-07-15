using UnityEngine;
using System.Collections.Generic;

public class TargetMinimapMarkerManager : MonoBehaviour
{
    public static TargetMinimapMarkerManager Instance { get; private set; }

    [SerializeField] private GameObject markerPrefab;

    private readonly Dictionary<string, GameObject> activeMarkers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CreateMarkerForTarget(TargetData target)
    {
        if (target == null || activeMarkers.ContainsKey(target.targetId)) return;

        GameObject marker = Instantiate(markerPrefab, target.transform.position, Quaternion.identity);
        activeMarkers[target.targetId] = marker;

        // ✅ AÑADIR ClickableTarget
        var clickable = marker.GetComponent<ClickableTarget>();
        if (clickable == null)
        {
            Debug.LogWarning($"⚠️ El marcador del target {target.targetId} no tiene ClickableTarget");
            return;
        }
        clickable.SetTargetId(target.targetId);

        Debug.Log($"📌 Marcador generado y ClickableTarget asignado para {target.targetId}");
    }

}
