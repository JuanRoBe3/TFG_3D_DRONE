using System.Collections.Generic;
using UnityEngine;

public class SearchZoneReplicaManager : MonoBehaviour
{
    [Header("Prefab visual (debe tener malla + opcionalmente SearchingZoneReplica)")]
    [SerializeField] private GameObject zonePrefab;

    private readonly Dictionary<string, GameObject> zones = new();

    void Awake()
    {
        Debug.Log("🧠 [SearchZoneReplicaManager] AWAKE");

        if (zonePrefab == null)
            Debug.LogError("❌ zonePrefab NO asignado en el Inspector.");
        else
            Debug.Log("✅ zonePrefab asignado correctamente.");

        bool hadPendingZones = false;

        if (PendingZoneBuffer.Instance != null)
        {
            hadPendingZones = PendingZoneBuffer.Instance.HasPendingZones();
            PendingZoneBuffer.Instance.ConsumeAll(OnZoneReceived);
        }
        else
        {
            Debug.LogWarning("⚠️ No hay instancia de PendingZoneBuffer disponible.");
        }

        //— Si NO había zonas pendientes, avisamos igualmente para detener el loop —
        if (!hadPendingZones) NotifyBroadcaster();
    }

    private void OnZoneReceived(string json)
    {
        Debug.Log("📥 [ReplicaManager] Mensaje recibido por MQTT");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("❌ JSON vacío");
            return;
        }

        SearchZoneData data;
        try { data = JsonUtility.FromJson<SearchZoneData>(json); }
        catch { Debug.LogError($"❌ Error al parsear JSON:\n{json}"); return; }

        if (data == null)
        {
            Debug.LogError("❌ JsonUtility devolvió null");
            return;
        }

        if (zones.ContainsKey(data.id))
        {
            Debug.LogWarning($"⚠️ Zona duplicada ignorada: {data.id}");
            return;
        }

        GameObject go = Instantiate(zonePrefab, transform);
        if (go == null)
        {
            Debug.LogError("❌ Error al instanciar zonePrefab");
            return;
        }

        go.name = $"SearchZone_{data.id}";
        Debug.Log($"🧩 Zona instanciada → Nombre: {go.name}");

        var replica = go.GetComponent<SearchingZoneReplica>();
        if (replica != null)
        {
            replica.Init(data);
        }
        else
        {
            go.transform.position = data.center.ToUnityVector3();
            go.transform.localScale = data.size.ToUnityVector3();
        }

        zones[data.id] = go;
        Debug.Log($"📦 Zona registrada. Total actual: {zones.Count}");

        NotifyBroadcaster();
    }

    private void NotifyBroadcaster()
    {
        var broadcaster = FindObjectOfType<PilotReadyBroadcaster>();
        if (broadcaster != null)
        {
            broadcaster.NotifyZonesReceived();
        }
    }

    void OnDestroy()
    {
        zones.Clear();  // evita duplicados si vuelves a entrar en la escena en la misma sesión
    }
}
