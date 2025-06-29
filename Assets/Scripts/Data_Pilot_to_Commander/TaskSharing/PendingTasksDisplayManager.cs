using UnityEngine;
using System.Collections.Generic;
using System;

public class PendingTasksDisplayManager : MonoBehaviour
{
    [Header("Prefabs y contenedor")]
    public GameObject taskPrefab;
    public Transform contentParent;

    private readonly List<GameObject> instantiatedItems = new();
    private readonly Queue<string> pendingJsonQueue = new();
    private List<TaskSummary> lastReceivedTasks = new();

    private const string pendingTasksTopic = MQTTConstants.PendingTasksTopic;
    private const string pendingRequestTopic = MQTTConstants.PendingTasksRequestTopic;

    private Action<string> cachedHandler;

    private void Awake()
    {
        cachedHandler = EnqueueJson;
    }

    private void OnEnable()
    {
        Debug.Log("🟢 PendingTasksDisplayManager habilitado.");

        if (MQTTClient.Instance != null && MQTTClient.Instance.GetClient()?.IsConnected == true)
        {
            Debug.Log("✅ Cliente MQTT conectado. Suscribiendo y solicitando tareas...");
            MQTTClient.Instance.RegisterHandler(pendingTasksTopic, cachedHandler);
            RequestPendingTasks();
        }
        else
        {
            Debug.Log("⏳ Cliente MQTT no conectado. Se registrará OnConnected...");
            MQTTClient.Instance.RegisterHandler(pendingTasksTopic, cachedHandler);
            MQTTClient.Instance.OnConnected += HandleMQTTConnected;
        }

        DroneSelectionManager.OnDroneChanged += UpdateTaskDisplay;
    }

    private void OnDisable()
    {
        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(pendingTasksTopic);
            MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        }

        DroneSelectionManager.OnDroneChanged -= UpdateTaskDisplay;
    }

    private void Start()
    {
        if (taskPrefab == null)
        {
            Debug.LogError("❌ taskPrefab no está asignado.");
            return;
        }

        if (!taskPrefab.TryGetComponent(out PendingTaskSummaryUI _))
            Debug.LogError("❌ taskPrefab no tiene PendingTaskSummaryUI.");
        else
            Debug.Log("✅ taskPrefab verificado.");
    }

    private void EnqueueJson(string json)
    {
        lock (pendingJsonQueue)
        {
            pendingJsonQueue.Enqueue(json);
        }
    }

    private void Update()
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

    private void ProcessTasks(string json)
    {
        Debug.Log($"📥 Procesando JSON: {json}");

        TaskSummaryListWrapper wrapper;
        try
        {
            wrapper = JsonUtility.FromJson<TaskSummaryListWrapper>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Deserialización fallida: " + ex.Message);
            return;
        }

        if (wrapper?.tasks == null || wrapper.tasks.Count == 0)
        {
            Debug.Log("📭 No hay tareas en el payload.");
            return;
        }

        lastReceivedTasks = wrapper.tasks;
        UpdateTaskDisplay(DroneSelectionManager.Instance.GetSelectedDrone());
    }

    private void UpdateTaskDisplay(DroneData selectedDrone)
    {
        if (selectedDrone == null)
        {
            Debug.Log("⚠️ Piloto sin dron seleccionado.");
            return;
        }

        foreach (var go in instantiatedItems) Destroy(go);
        instantiatedItems.Clear();

        string droneId = selectedDrone.droneName;

        var filtered = lastReceivedTasks.FindAll(t =>
            t.status == "To be executed" &&
            string.Equals(t.drone, droneId, StringComparison.OrdinalIgnoreCase));

        Debug.Log($"🎯 {filtered.Count} tareas para dron «{droneId}»");

        foreach (var task in filtered)
        {
            GameObject go = Instantiate(taskPrefab, contentParent);
            if (go.TryGetComponent(out PendingTaskSummaryUI ui))
                ui.Setup(task);

            instantiatedItems.Add(go);
        }
    }

    private void RequestPendingTasks()
    {
        Debug.Log("📤 [Pilot] Enviando petición de tareas pendientes…");

        var publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
        publisher.PublishMessage(pendingRequestTopic, "request_pending_tasks");
    }

    private void HandleMQTTConnected()
    {
        Debug.Log("🔄 Cliente MQTT conectado tras espera. Solicitando tareas...");
        MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        RequestPendingTasks();
    }
}