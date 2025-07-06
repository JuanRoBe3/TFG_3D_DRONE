using UnityEngine;

public class SearchZonePublisher : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;   // Prefab visual que se usó localmente

    /// <summary>
    /// Llama a este método desde tu UI o input de creación de zonas.
    /// </summary>
    public void PublishZone(Vector3 center, Vector3 size)
    {
        var data = new SearchZoneData
        {
            id = System.Guid.NewGuid().ToString(),
            center = new SerializableVector3(center),
            size = new SerializableVector3(size)
        };

        // 1️⃣ Registrar
        SearchZoneRegistry.Register(data);

        // 2️⃣ Publicar por MQTT
        var client = MQTTClient.Instance?.GetClient();
        if (client != null && client.IsConnected)
        {
            string json = JsonUtility.ToJson(data);
            new MQTTPublisher(client).PublishMessage(MQTTConstants.SearchingZone, json);
            Debug.Log($"📤 Zona publicada por MQTT: {json}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo publicar por MQTT. Cliente desconectado.");
        }
    }


}
