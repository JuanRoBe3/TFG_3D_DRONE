using UnityEngine;
using System.Collections.Generic;

public class TaskSelectionManager : MonoBehaviour
{
    public static TaskSelectionManager Instance { get; private set; }

    private SelectableTaskItem selectedItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectTask(SelectableTaskItem newItem, TaskSummary taskData)
    {
        // 🔄 Deselecciona el anterior
        if (selectedItem != null)
            selectedItem.Deselect();

        // ✅ Actualiza la selección
        selectedItem = newItem;
        selectedItem.Select();

        // 🧠 Registra la task seleccionada
        PendingTasksDisplayManager.SelectTaskExternally(taskData, newItem);
    }
}
