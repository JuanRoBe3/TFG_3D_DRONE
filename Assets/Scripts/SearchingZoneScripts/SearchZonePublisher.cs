using UnityEngine;

public class SearchZonePublisher : MonoBehaviour
{
    [SerializeField] private GameObject zonePrefab;   // Prefab visual que se usó localmente

    /// <summary>
    /// Llama a este método desde tu UI o input de creación de zonas.
    /// </summary>
    public void PublishZone(Vector3 center, Vector3 size)
    {
        // 1️⃣ Crear estructura de datos
        var data = new SearchZoneData
        {
            id = System.Guid.NewGuid().ToString(),
            center = new SerializableVector3(center),
            size = new SerializableVector3(size)
        };

        // 2️⃣ Registrar en memoria para reenviar si el piloto entra tarde
        if (SearchZoneRegistry.Instance == null)
        {
            Debug.LogError("❌ No se puede registrar la zona: SearchZoneRegistry.Instance es NULL. ¿Está en escena?");
        }
        else
        {
            SearchZoneRegistry.Instance.Register(data);
        }

        // 3️⃣ Publicar por MQTT
        MQTTClient.EnsureExists();
        var mqtt = MQTTClient.Instance.GetClient();

        if (mqtt != null && mqtt.IsConnected)
        {
            var json = JsonUtility.ToJson(data);
            new MQTTPublisher(mqtt).PublishMessage(MQTTConstants.SearchingZone, json);
            Debug.Log($"📤 Zona publicada: {json}");
        }
        else
        {
            Debug.LogWarning("⚠️ MQTT no conectado. Zona NO publicada todavía.");
        }
    }
}
