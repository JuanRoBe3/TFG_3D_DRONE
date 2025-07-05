using System.Collections.Generic;
using UnityEngine;

public class SearchZoneReplicaManager : MonoBehaviour
{
    [Header("Prefab visual (Mesh + SearchingZoneReplica)")]
    [SerializeField] private GameObject zonePrefab;

    private readonly Dictionary<string, GameObject> zones = new();

    void Awake()
    {
        MQTTClient.EnsureExists();
        MQTTClient.Instance.RegisterHandler(MQTTConstants.SearchingZone, OnZoneReceived);
    }

    private void OnZoneReceived(string json)
    {
        SearchZoneData data = JsonUtility.FromJson<SearchZoneData>(json);
        if (data == null) { Debug.LogError("❌ JSON malformado"); return; }

        if (zones.ContainsKey(data.id)) return;

        GameObject go = Instantiate(zonePrefab ?? FallbackCube(), transform);
        go.name = $"SearchZone_{data.id}";

        var rep = go.GetComponent<SearchingZoneReplica>();
        if (rep != null) rep.Init(data);
        else
        {
            go.transform.position = data.center.ToUnityVector3();
            go.transform.localScale = data.size.ToUnityVector3();
        }

        zones[data.id] = go;
    }

    private GameObject FallbackCube()
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = Color.red;
        return cube;
    }
}
