using UnityEngine;

[System.Serializable]
public struct SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToUnityVector3() => new Vector3(x, y, z);
    public override string ToString()
    {
        return $"({x:F2}, {y:F2}, {z:F2})"; // con 2 decimales
    }
}