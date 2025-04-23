using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PendingTaskSummaryUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescriptionText;
    public TextMeshProUGUI droneText;
    public TextMeshProUGUI statusText;
    public Image statusColorImage;
    public Image droneIconImage;

    /// <summary>
    /// Rellena el UI con los datos de una tarea recibida por MQTT
    /// </summary>
    /// <param name="summary">Resumen de la tarea</param>
    public void Setup(TaskSummary summary)
    {
        Debug.Log($"🛠 Configurando tarea: {summary.title}");

        if (summary == null)
        {
            Debug.LogWarning("⚠️ No se puede configurar la UI: TaskSummary es null.");
            return;
        }

        taskNameText.text = summary.title;
        taskDescriptionText.text = summary.description;
        droneText.text = summary.drone;
        statusText.text = summary.status;

        if (statusColorImage != null)
            statusColorImage.color = TaskStatusColor.GetColorForStatus(summary.status);

        // Buscar el icono del dron por su nombre
        var droneData = DroneSelectionManager.Instance.availableDrones
            .Find(d => d.droneName == summary.drone);

        if (droneData != null && droneIconImage != null)
        {
            droneIconImage.sprite = droneData.icon;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró el icono del dron '{summary.drone}'");
        }
    }
}
