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
    private SelectableTaskItem selectableItem;

    void Awake()
    {
        selectableItem = GetComponent<SelectableTaskItem>();
        if (selectableItem == null)
            Debug.LogError("❌ No se encontró SelectableTaskItem.");
    }

    public void Setup(TaskSummary summary)
    {
        this.summary = summary;

        if (summary == null)
        {
            Debug.LogWarning("⚠️ TaskSummary es null en Setup.");
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
            droneIconImage.sprite = droneData.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (summary == null)
        {
            Debug.LogWarning("⚠️ Click sin tarea.");
            return;
        }

        Debug.Log($"🖱️ Tarea seleccionada: {summary.title}");
        PendingTasksDisplayManager.SelectTaskExternally(summary, selectableItem);
    }
}
