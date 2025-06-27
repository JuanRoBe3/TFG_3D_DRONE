using UnityEngine;

public class ZoneReplicaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;    // mismo prefab
    [SerializeField] private bool concentric = false;  // opcional: evita duplicados

    void Awake() => ZoneSyncManager.OnZoneCreated += Spawn;
    void OnDestroy() => ZoneSyncManager.OnZoneCreated -= Spawn;

    void Spawn(Vector3 center, Vector3 size)
    {
        if (concentric)            // no instanciar dos veces la misma
        {
            foreach (Transform t in transform)
                if (Vector3.Distance(t.position, center) < 0.1f) return;
        }

        GameObject z = Instantiate(zonePrefab, center, Quaternion.identity, transform);
        z.transform.localScale = size;
        z.layer = LayerMask.NameToLayer("SearchZone");
    }
}
