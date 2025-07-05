using System.Collections.Generic;
using UnityEngine;

public class PendingZoneBuffer : MonoBehaviour
{
    public static PendingZoneBuffer Instance { get; private set; }

    private readonly List<SearchZoneData> buffer = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("📦 [PendingZoneBuffer] Iniciado correctamente");
    }

    public void Add(SearchZoneData zone)
    {
        if (zone == null)
        {
            Debug.LogWarning("⚠️ Intento de añadir zona nula al buffer");
            return;
        }

        buffer.Add(zone);
        Debug.Log($"➕ Zona añadida al buffer: {zone.id}. Total actual: {buffer.Count}");
    }

    public bool HasPendingZones() => buffer.Count > 0;

    public void ConsumeAll(System.Action<string> callback)
    {
        Debug.Log($"🔁 Procesando {buffer.Count} zonas del buffer...");

        foreach (var zone in buffer)
        {
            string json = JsonUtility.ToJson(zone);
            callback?.Invoke(json);
        }

        buffer.Clear();
    }
}
