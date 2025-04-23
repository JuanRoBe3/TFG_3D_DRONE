// Assets/Scripts/Utils/TaskUtils.cs

public static class TaskUtils
{
    public static bool AreSameTask(TaskData a, TaskData b)
    {
        return a != null && b != null && a.id == b.id;
    }
}
