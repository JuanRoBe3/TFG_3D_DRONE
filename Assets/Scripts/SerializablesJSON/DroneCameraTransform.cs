using UnityEngine;

[System.Serializable]
public class DroneCameraTransform
{
    public string id;                   // 👈 nuevo
    public SerializableVector3 pos;
    public SerializableQuaternion rot;
}
