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
    private readonly Queue<string> pendingJsonQueue = new(); // 🔁 Cola de JSONs a procesar

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
        MQTTClient.Instance.RegisterHandler(MQTTConstants.PendingTasksTopic, EnqueueJson);

        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.PendingTasksRequestTopic, "request_pending_tasks");
    }

    void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(MQTTConstants.PendingTasksTopic);
    }

    void EnqueueJson(string json)
    {
        // 🔁 Guarda el mensaje para que se procese en Update()
        lock (pendingJsonQueue)
        {
            pendingJsonQueue.Enqueue(json);
        }
    }

    void Update()
    {
        // Procesamos las tareas pendientes solo en el hilo principal
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

        foreach (var go in instantiatedItems)
            Destroy(go);
        instantiatedItems.Clear();

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

        foreach (var task in wrapper.tasks)
        {
            Debug.Log($"🔧 Instanciando prefab para tarea: {task.title}");

            GameObject go = Instantiate(taskPrefab, contentParent);
            var summaryUI = go.GetComponent<PendingTaskSummaryUI>();

            if (summaryUI != null)
            {
                summaryUI.Setup(task);
                Debug.Log($"✅ Setup() completado para tarea: {task.title}");
            }
            else
            {
                Debug.LogWarning("⚠️ Prefab sin PendingTaskSummaryUI.");
            }

            instantiatedItems.Add(go);
        }
    }
}
