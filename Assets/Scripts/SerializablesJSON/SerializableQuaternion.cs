using UnityEngine;

[System.Serializable]
public struct SerializableQuaternion
{
    public float x, y, z, w;
    public SerializableQuaternion(Quaternion q) { x = q.x; y = q.y; z = q.z; w = q.w; }
    public Quaternion ToUnityQuaternion() => new Quaternion(x, y, z, w);
}