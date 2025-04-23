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

    /// <summary>
    /// Rellena el UI con los datos de una tarea recibida por MQTT
    /// </summary>
    /// <param name="summary">Resumen de la tarea</param>
    public void Setup(TaskSummary summary)
    {
        Debug.Log($"COLIFLOOOOOOR");
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
    }
}
