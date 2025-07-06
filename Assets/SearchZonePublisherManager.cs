using UnityEngine;
using System.Collections.Generic;
using System;

public class SearchZonePublisherManager : MonoBehaviour
{
    private void OnEnable()
    {
        // 📩 El piloto puede pedir zonas explícitamente
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingSearchZonesRequestTopic, OnPendingZonesRequestReceived);
    }

    private void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingSearchZonesRequestTopic, OnPendingZonesRequestReceived);
    }

    private void Start()
    {
        // ✅ Igual que en tareas: publicar todas las zonas al iniciar escena
        PublishPendingZones();
    }

    private void OnPendingZonesRequestReceived(string _)
    {
        Debug.Log("📨 Petición de zonas recibida por MQTT desde el piloto.");
        PublishPendingZones();
    }

    public void PublishPendingZones()
    {
        // ✅ Paso 1: obtener la lista correcta
        List<SearchZoneSummary> summaries = SearchZoneRegistry.GetAllZones();

        // ✅ Paso 2: crear wrapper para JsonUtility
        var wrapper = new SearchZoneSummaryListWrapper { zones = summaries };

        // ✅ Paso 3: serializar y publicar por MQTT
        string json = JsonUtility.ToJson(wrapper);
        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingSearchZonesTopic, json);

        Debug.Log($"📤 Publicadas {summaries.Count} zonas pendientes");
    }


    public void RegisterZone(SearchZoneData zone)
    {
        if (zone == null || string.IsNullOrEmpty(zone.id))
        {
            zone.id = Guid.NewGuid().ToString();
            Debug.Log($"🆕 Generado ID para nueva zona: {zone.id}");
        }

        SearchZoneRegistry.Register(zone);
        Debug.Log($"🧾 Zona registrada → ID: {zone.id}");

        // Re-publicamos al registrar una nueva
        PublishPendingZones();
    }
}
