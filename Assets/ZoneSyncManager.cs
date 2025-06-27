using UnityEngine;

public static class ZoneSyncManager
{
    public static System.Action<Vector3, Vector3> OnZoneCreated; // center, size

    public static void Publish(Vector3 center, Vector3 size)
    {
        // TODO: sustituir por envío MQTT si procede
        OnZoneCreated?.Invoke(center, size);
    }
}
