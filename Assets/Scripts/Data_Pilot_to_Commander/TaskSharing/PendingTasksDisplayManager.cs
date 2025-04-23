using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PendingTasksDisplayManager : MonoBehaviour
{
    [Header("Prefabs y contenedor")]
    public GameObject taskPrefab;
    public Transform contentParent;

    private readonly List<GameObject> instantiatedItems = new();
    private readonly Queue<string> pendingJsonQueue = new();
    private List<TaskSummary> lastReceivedTasks = new(); // 🔁 Almacena última tanda

    void Start()
    {
        if (taskPrefab == null)
        {
            Debug.LogError("❌ taskPrefab no está asignado.");
            return;
        }

        var summaryUI = taskPrefab.GetComponent<PendingTaskSummaryUI>();
        if (summaryUI == null)
            Debug.LogError("❌ taskPrefab no tiene PendingTaskSummaryUI.");
        else
            Debug.Log("✅ taskPrefab verificado correctamente.");
    }

    void OnEnable()
    {
        DroneSelectionManager.OnDroneChanged += UpdateTaskDisplay;

        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksTopic, EnqueueJson);

        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingTasksRequestTopic, "request_pending_tasks");
    }

    void OnDisable()
    {
        DroneSelectionManager.OnDroneChanged -= UpdateTaskDisplay;
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksTopic);
    }

    void EnqueueJson(string json)
    {
        lock (pendingJsonQueue)
        {
            pendingJsonQueue.Enqueue(json);
        }
    }

    void Update()
    {
        while (pendingJsonQueue.Count > 0)
        {
            string json;
            lock (pendingJsonQueue)
            {
                json = pendingJsonQueue.Dequeue();
            }
            ProcessTasks(json);
        }
    }

    void ProcessTasks(string json)
    {
        Debug.Log("📥 Procesando JSON en main thread: " + json);

        TaskSummaryListWrapper wrapper;
        try
        {
            wrapper = JsonUtility.FromJson<TaskSummaryListWrapper>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error al deserializar: " + ex.Message);
            return;
        }

        if (wrapper?.tasks == null || wrapper.tasks.Count == 0)
        {
            Debug.Log("📭 No hay tareas recibidas.");
            return;
        }

        Debug.Log($"📦 {wrapper.tasks.Count} tareas recibidas.");
        lastReceivedTasks = wrapper.tasks; // 💾 Guardamos tareas recibidas

        UpdateTaskDisplay(DroneSelectionManager.Instance.GetSelectedDrone());
    }

    void UpdateTaskDisplay(DroneData selectedDrone)
    {
        if (selectedDrone == null)
        {
            Debug.Log("⚠️ Ningún dron seleccionado.");
            return;
        }

        string selectedDroneId = selectedDrone.droneName;

        foreach (var go in instantiatedItems)
            Destroy(go);
        instantiatedItems.Clear();

        var filtered = lastReceivedTasks.FindAll(task =>
            task.status == "To be executed" &&
            !string.IsNullOrEmpty(task.drone) &&
            task.drone.StartsWith(selectedDroneId)
        );

        Debug.Log($"🎯 Mostrando {filtered.Count} tareas para {selectedDroneId}");

        foreach (var task in filtered)
        {
            GameObject go = Instantiate(taskPrefab, contentParent);
            var summaryUI = go.GetComponent<PendingTaskSummaryUI>();

            if (summaryUI != null)
                summaryUI.Setup(task);

            instantiatedItems.Add(go);
        }
    }
}
