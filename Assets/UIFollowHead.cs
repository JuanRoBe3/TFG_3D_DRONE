using UnityEngine;

public class UIFollowHead : MonoBehaviour
{
    [SerializeField] Transform head;   // CenterEyeAnchor
    [SerializeField] float distance = 2f;
    [SerializeField] Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        transform.position = head.position + head.forward * distance + offset;
        transform.rotation = Quaternion.LookRotation(head.forward, Vector3.up);
    }
}
