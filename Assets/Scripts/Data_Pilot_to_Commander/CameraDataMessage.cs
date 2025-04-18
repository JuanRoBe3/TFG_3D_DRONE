using UnityEngine;

[System.Serializable]
public class CameraDataMessage
{
    public Vector3 firstPersonPos;
    public Quaternion firstPersonRot;
    public Vector3 topDownPos;
    public Quaternion topDownRot;

    public CameraDataMessage(Vector3 fpPos, Quaternion fpRot, Vector3 tdPos, Quaternion tdRot)
    {
        firstPersonPos = fpPos;
        firstPersonRot = fpRot;
        topDownPos = tdPos;
        topDownRot = tdRot;
    }
}
