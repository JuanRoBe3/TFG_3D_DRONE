using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyButtonHandler : MonoBehaviour
{
    public static FlyButtonHandler Instance { get; private set; }

    [Header("Referencias necesarias")]
    [SerializeField] private PendingTasksDisplayManager tasksManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnFlyButtonPressed()
    {
        // Obtener la tarea y el dron seleccionados
        TaskSummary selectedTask = tasksManager.GetSelectedTask();
        DroneData selectedDrone = DroneSelectionManager.Instance.GetSelectedDrone();

        // Validaciones
        if (selectedTask == null)
        {
            Debug.LogWarning("⚠️ No se ha seleccionado ninguna tarea.");
            return;
        }

        if (selectedDrone == null)
        {
            Debug.LogWarning("⚠️ No se ha seleccionado ningún dron.");
            return;
        }

        // Construir mensaje MQTT
        var msg = new TaskSelectionMessage
        {
            taskId = selectedTask.id,
            droneId = selectedDrone.droneName,
            newStatus = "Executing"
        };

        string json = JsonUtility.ToJson(msg);

        // Enviar por MQTT
        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.SelectedTaskTopic, json);

        Debug.Log($"📤 MQTT publicado desde botón Fly: {json}");

        // Guardar contexto para siguiente escena
        SelectedTaskContext.Instance?.SetTask(selectedTask);
        SelectedDroneHolder.SetDrone(selectedDrone);

        // Cambiar de escena
        SceneLoader.LoadPilotUI();
    }
}
