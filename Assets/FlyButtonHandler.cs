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
        // 1. Obtener selección
        TaskSummary selectedTask = tasksManager.GetSelectedTask();
        DroneData selectedDrone = DroneSelectionManager.Instance.GetSelectedDrone();

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

        // 2. Publicar MQTT (task/selected)
        var msg = new TaskSelectionMessage
        {
            taskId = selectedTask.id,
            droneId = selectedDrone.droneName,
            newStatus = "Executing"
        };
        string json = JsonUtility.ToJson(msg);
        new MQTTPublisher(MQTTClient.Instance.GetClient())
            .PublishMessage(MQTTConstants.SelectedTaskTopic, json);

        Debug.Log($"📤 MQTT publicado desde botón Fly: {json}");

        // 3. Guardar datos para la escena de vuelo (PlayerPrefs)
        PlayerPrefs.SetString("SelectedTaskId", selectedTask.id);
        PlayerPrefs.SetString("SelectedDroneId", selectedDrone.droneName);
        PlayerPrefs.Save();

        // 4. Cambiar de escena
        SceneLoader.LoadPilotUI();
    }
}
