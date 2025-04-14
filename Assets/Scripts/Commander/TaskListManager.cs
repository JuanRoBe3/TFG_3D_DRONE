using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : MonoBehaviour
{
    [Header("Prefabs y referencias")]
    public GameObject taskItemPrefab;
    public Transform contentParent;                   // Content del ScrollView
    public RectTransform scrollViewRectTransform;     // RectTransform del ScrollView

    [Header("Altura relativa de las tareas")]
    [Range(0.1f, 1f)]
    public float taskHeightPercentage = 0.3f;

    private void Start()
    {
        // Al iniciar, ajusta cualquier tarea existente (por si hay alguna manual)
        AdjustAllTaskItems();
    }

    private void OnRectTransformDimensionsChange()
    {
        // Se llama automáticamente si cambia el tamaño del objeto con RectTransform asignado
        AdjustAllTaskItems();
    }

    /// <summary>
    /// Añade una nueva tarea a la lista con valores de prueba.
    /// </summary>
    public void AddTask()
    {
        GameObject item = Instantiate(taskItemPrefab, contentParent);
        AdjustTaskItemHeight(item);

        // Valores de ejemplo (puedes sustituir esto por un formulario luego)
        TaskItemUI taskUI = item.GetComponent<TaskItemUI>();
        if (taskUI != null)
        {
            string randomName = "Tarea " + Random.Range(1, 100);
            string status = "Pendiente";
            taskUI.Setup(randomName, status);
        }
    }

    /// <summary>
    /// Ajusta la altura de un único item.
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
}
