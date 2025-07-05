using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registra todas las zonas de búsqueda creadas en esta sesión.
/// Se utiliza para reenviarlas por MQTT si un piloto entra tarde.
/// </summary>
public class SearchZoneRegistry : MonoBehaviour
{
    public static SearchZoneRegistry Instance { get; private set; }

    private readonly List<SearchZoneData> zones = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Duplicado de SearchZoneRegistry detectado. Se destruye esta instancia.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("✅ SearchZoneRegistry inicializado correctamente.");

        // ❗ Si en el futuro necesitas que esto persista entre escenas:
        // DontDestroyOnLoad(gameObject);
    }

    public void Register(SearchZoneData data)
    {
        if (data == null)
        {
            Debug.LogError("❌ SearchZoneData es null. No se puede registrar.");
            return;
        }

        Debug.Log($"🧩 Zona registrada: ID={data.id}, Pos={data.center}, Size={data.size}");
        zones.Add(data);
        DebugPrintAll();
    }

    public void DebugPrintAll()
    {
        Debug.Log($"🗂️ Total zonas registradas: {zones.Count}");
        foreach (var z in zones)
            Debug.Log($"🧾 {z.id} | Pos: {z.center} | Size: {z.size}");
    }

    public IReadOnlyList<SearchZoneData> GetAll() => zones;
}
