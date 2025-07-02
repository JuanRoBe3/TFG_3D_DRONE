using UnityEngine;

public class SelectedTaskContext : MonoBehaviour
{
    public static SelectedTaskContext Instance { get; private set; }

    public TaskSummary CurrentTask { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetTask(TaskSummary summary)
    {
        CurrentTask = summary;
        Debug.Log($"📌 Tarea guardada en SelectedTaskContext: {summary.title}");
    }
}
