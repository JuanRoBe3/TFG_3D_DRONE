using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    public TextMeshProUGUI taskNameText;
    public TextMeshProUGUI statusText;

    public void Setup(string taskName, string status)
    {
        taskNameText.text = taskName;
        statusText.text = status;
    }
}
