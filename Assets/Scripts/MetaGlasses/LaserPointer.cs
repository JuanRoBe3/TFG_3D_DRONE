using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public LineRenderer lr;
    public float length = 3f;

    void Update()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + transform.forward * length);
    }
}
