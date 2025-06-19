using UnityEngine;

public class DroneCameraFollower : MonoBehaviour
{
    Transform target;
    [SerializeField] Vector3 offsetPosition = Vector3.zero;   // regula si quieres cabina o vista externa
    [SerializeField] Vector3 offsetEuler = Vector3.zero;

    public void SetTarget(Transform newTarget) => target = newTarget;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.TransformPoint(offsetPosition);
        transform.rotation = target.rotation * Quaternion.Euler(offsetEuler);
    }
}
