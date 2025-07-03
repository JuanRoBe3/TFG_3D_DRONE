using System.Collections.Generic;

public static class TaskRegistry
{
    private static List<TaskSummary> allTasks = new();

    public static TaskSummary GetTaskById(string id) =>
        allTasks.Find(t => t.id == id);

    public static void SetTasks(List<TaskSummary> tasks) =>
        allTasks = tasks;

    public static List<TaskSummary> GetAllTasks() => allTasks;
}
