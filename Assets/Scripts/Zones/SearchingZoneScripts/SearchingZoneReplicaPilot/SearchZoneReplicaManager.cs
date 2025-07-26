using UnityEngine;

public class SearchZoneReplicaManager : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private SearchRouteGenerator routeGenerator; // ✅ Añadido

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

        // ✅ Generar ruta visual en la escena del Piloto
        if (routeGenerator != null)
        {
            routeGenerator.GenerateRouteForZone(go);
            Debug.Log($"📍 Ruta generada para la zona: {go.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró SearchRouteGenerator en escena del piloto.");
        }
    }
}
