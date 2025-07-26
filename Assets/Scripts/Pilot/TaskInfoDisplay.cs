using UnityEngine;
using TMPro;

public class TaskInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskInfoText;

    void Start()
    {
        string title = SelectedTaskHolder.TaskTitle;
        string description = SelectedTaskHolder.TaskDescription;

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
        {
            taskInfoText.text = $"{title}: {description}";
        }
        else
        {
            taskInfoText.text = "No task selected.";
        }
    }
}
