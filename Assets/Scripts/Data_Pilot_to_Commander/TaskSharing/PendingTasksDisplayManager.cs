using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

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

    public static PendingTasksDisplayManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        cachedHandler = EnqueueJson;
        Debug.Log("📦 PendingTasksDisplayManager → Awake completado.");
    }

    private void OnEnable()
    {
        Debug.Log("🟢 PendingTasksDisplayManager habilitado.");

        if (MQTTClient.Instance != null && MQTTClient.Instance.GetClient()?.IsConnected == true)
        {
            Debug.Log("✅ Cliente MQTT ya conectado. Suscribiendo a tareas pendientes y solicitando...");
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
        Debug.Log("🔴 PendingTasksDisplayManager deshabilitado.");

        if (MQTTClient.Instance != null)
        {
            MQTTClient.Instance.UnregisterHandler(pendingTasksTopic);
            MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        }

        DroneSelectionManager.OnDroneChanged -= UpdateTaskDisplay;
    }

    private void Start()
    {
        Debug.Log("🚀 Start de PendingTasksDisplayManager.");

        if (taskPrefab == null)
        {
            Debug.LogError("❌ taskPrefab no está asignado.");
            return;
        }

        if (!taskPrefab.TryGetComponent(out PendingTaskSummaryUI _))
            Debug.LogError("❌ taskPrefab no tiene PendingTaskSummaryUI.");
        else
            Debug.Log("✅ taskPrefab verificado correctamente.");
    }

    private void EnqueueJson(string json)
    {
        Debug.Log($"📩 JSON recibido por MQTT: {json}");

        lock (pendingJsonQueue)
        {
            pendingJsonQueue.Enqueue(json);
            Debug.Log("📥 JSON encolado para procesar.");
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
                Debug.Log($"🧩 JSON dequeued para procesar: {json}");
            }
            ProcessTasks(json);
        }
    }

    private void ProcessTasks(string json)
    {
        Debug.Log($"🔍 Intentando deserializar: {json}");

        TaskSummaryListWrapper wrapper;
        try
        {
            wrapper = JsonUtility.FromJson<TaskSummaryListWrapper>(json);
            Debug.Log("✅ JSON deserializado correctamente.");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error en deserialización: " + ex.Message);
            return;
        }

        if (wrapper?.tasks == null || wrapper.tasks.Count == 0)
        {
            Debug.Log("📭 No hay tareas dentro del wrapper deserializado.");
            return;
        }

        Debug.Log($"📋 {wrapper.tasks.Count} tareas recibidas. Actualizando visual…");

        lastReceivedTasks = wrapper.tasks;
        UpdateTaskDisplay(DroneSelectionManager.Instance.GetSelectedDrone());
    }

    private void UpdateTaskDisplay(DroneData selectedDrone)
    {
        Debug.Log("🔁 Ejecutando UpdateTaskDisplay…");

        if (selectedDrone == null)
        {
            Debug.Log("⚠️ Dron seleccionado es null.");
            return;
        }

        Debug.Log($"🛩️ Dron seleccionado: {selectedDrone.droneName}");

        foreach (var go in instantiatedItems) Destroy(go);
        instantiatedItems.Clear();

        string droneId = selectedDrone.droneName;

        var filtered = lastReceivedTasks.FindAll(t =>
            t.status == "To be executed" &&
            string.Equals(t.drone, droneId, StringComparison.OrdinalIgnoreCase));

        Debug.Log($"🎯 Filtrado: {filtered.Count} tareas para el dron «{droneId}»");

        foreach (var task in filtered)
        {
            GameObject go = Instantiate(taskPrefab, contentParent);
            if (go.TryGetComponent(out PendingTaskSummaryUI ui))
            {
                Debug.Log($"🧱 Instanciando UI para tarea: {task.title} (id: {task.id})");
                ui.Setup(task);
            }
            else
            {
                Debug.LogWarning("⚠️ Prefab instanciado no contiene PendingTaskSummaryUI.");
            }

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
        Debug.Log("🔌 Cliente MQTT reconectado. Lanzando petición de tareas pendientes...");
        MQTTClient.Instance.OnConnected -= HandleMQTTConnected;
        RequestPendingTasks();
    }

    // ⬇️ Lógica para selección manual

    public static void SelectTaskExternally(TaskSummary summary, SelectableTaskItem visual)
    {
        Debug.Log("🖱️ SelectTaskExternally con imagen visual.");
        Instance?.HandleTaskChosen(summary, visual);
    }

    private TaskSummary selectedTask;

    private SelectableTaskItem selectedVisual;

    private void HandleTaskChosen(TaskSummary summary, SelectableTaskItem visual = null)
    {
        if (summary == null)
        {
            Debug.LogWarning("⚠️ No se puede seleccionar una tarea nula.");
            return;
        }

        selectedTask = summary;

        // 🔁 Actualiza visualmente
        if (selectedVisual != null)
            selectedVisual.Deselect();

        selectedVisual = visual;

        if (selectedVisual != null)
            selectedVisual.Select();

        Debug.Log($"✅ Tarea seleccionada visual y lógicamente: {summary.title}");
    }


    public TaskSummary GetSelectedTask()
    {
        return selectedTask;
    }
}
