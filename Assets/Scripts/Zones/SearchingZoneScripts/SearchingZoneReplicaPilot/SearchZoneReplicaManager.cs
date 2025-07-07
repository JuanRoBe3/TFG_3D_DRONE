using UnityEngine;

public class SearchZoneReplicaManager : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;

    public void OnZoneReceived(string json)
    {
        var data = JsonUtility.FromJson<SearchZoneData>(json);
        if (data == null)
        {
            Debug.LogError("❌ Zona malformada");
            return;
        }

        var go = Instantiate(zonePrefab, transform);
        go.name = $"SearchZone_{data.id}";
        go.transform.position = data.center.ToUnityVector3();
        go.transform.localScale = data.size.ToUnityVector3();
        Debug.Log($"🧱 Zona instanciada: {go.name}");
    }
}
