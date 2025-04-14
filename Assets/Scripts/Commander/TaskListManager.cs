using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : MonoBehaviour
{
    [Header("Prefabs y referencias")]
    public GameObject taskItemPrefab;                 // Prefab del Task_Item
    public Transform contentParent;                   // Content del ScrollView
    public RectTransform scrollViewRectTransform;     // RectTransform del ScrollView
    public TaskEditorUI taskEditorUI;                 // Panel flotante para crear tareas

    [Header("Altura relativa de las tareas")]
    [Range(0.1f, 1f)]
    public float taskHeightPercentage = 0.3f;

    private void Start()
    {
        AdjustAllTaskItems(); // Ajustar si ya hay tareas puestas
    }

    private void OnRectTransformDimensionsChange()
    {
        AdjustAllTaskItems(); // Redimensionar tareas cuando cambia el tamaño
    }

    /// <summary>
    /// Abre el panel de creación de tareas.
    /// </summary>
    public void OpenCreateTask()
    {
        taskEditorUI.Show((taskData) =>
        {
            // 1. Instanciar el prefab
            GameObject item = Instantiate(taskItemPrefab, contentParent);

            // 2. Ajustar altura
            AdjustTaskItemHeight(item);

            // 3. Visualizar la tarea
            TaskItemUI taskUI = item.GetComponent<TaskItemUI>();
            if (taskUI != null)
                taskUI.Setup(taskData, this);
        });
    }

    /// <summary>
    /// Ajusta la altura de una tarea concreta.
    /// </summary>
    private void AdjustTaskItemHeight(GameObject taskItem)
    {
        float containerHeight = scrollViewRectTransform.rect.height;
        float itemHeight = containerHeight * taskHeightPercentage;

        LayoutElement layout = taskItem.GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.preferredHeight = itemHeight;
            layout.flexibleHeight = 0;
        }
    }

    /// <summary>
    /// Ajusta la altura de todas las tareas visibles.
    /// </summary>
    public void AdjustAllTaskItems()
    {
        float containerHeight = scrollViewRectTransform.rect.height;
        float itemHeight = containerHeight * taskHeightPercentage;

        foreach (Transform child in contentParent)
        {
            LayoutElement layout = child.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.preferredHeight = itemHeight;
                layout.flexibleHeight = 0;
            }
        }
    }

    public void EditTask(TaskData existingData, TaskItemUI itemUI)
    {
        taskEditorUI.Show((updatedData) =>
        {
            // Actualizamos la referencia de datos
            existingData.title = updatedData.title;
            existingData.description = updatedData.description;
            existingData.status = updatedData.status;
            existingData.assignedDrone = updatedData.assignedDrone;

            // Volvemos a mostrar la UI con los nuevos datos
            itemUI.Setup(existingData, this);
        }, existingData);
    }


}
