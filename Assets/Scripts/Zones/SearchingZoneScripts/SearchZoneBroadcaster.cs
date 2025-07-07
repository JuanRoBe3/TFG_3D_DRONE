using UnityEngine;

public class SearchZoneBroadcaster : MonoBehaviour
{
    [SerializeField] private SearchZonePublisher publisher;

    void Start()
    {
        foreach (var zone in FindObjectsOfType<SearchingZone>())
        {
            Vector3 center = zone.transform.position;
            Vector3 size = zone.transform.localScale;
            publisher.PublishZone(center, size);
        }

        Debug.Log("🔄 Zonas republicadas al iniciar.");
    }
}

