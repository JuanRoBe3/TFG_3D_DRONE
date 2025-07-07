using UnityEngine;
using UnityEngine.UI;

public class ResizePendingTasksListener : MonoBehaviour
{
    public RectTransform scrollViewRectTransform;
    public float taskHeightPercentage = 0.3f; // Ajustable desde el Inspector
    public PendingTasksDisplayManager displayManager;

    private void OnRectTransformDimensionsChange()
    {
        ResizeAllTasks();
    }

    public void ResizeAllTasks()
    {
        if (displayManager == null || scrollViewRectTransform == null)
            return;

        float containerHeight = scrollViewRectTransform.rect.height;
        float itemHeight = containerHeight * taskHeightPercentage;

        foreach (Transform child in displayManager.contentParent)
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
