using UnityEngine;

public class ResizeTasksListener : MonoBehaviour
{
    public TaskListManager taskListManager;

    private void OnRectTransformDimensionsChange()
    {
        if (taskListManager != null)
        {
            taskListManager.AdjustAllTaskItems();
        }
    }
}
