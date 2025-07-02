using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PendingTaskSummaryUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias UI")]
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI taskDescriptionText;
    public TextMeshProUGUI droneText;
    public TextMeshProUGUI statusText;
    public Image statusColorImage;
    public Image droneIconImage;

    private TaskSummary summary;

    public void Setup(TaskSummary summary)
    {
        this.summary = summary;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (summary != null)
        {
            Debug.Log($"🖱️ Tarea seleccionada: {summary.title}");
            PendingTasksDisplayManager.SelectTaskExternally(summary);
        }
    }
}
