using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    public LayerMask detectableLayer;
    public float detectionRange = 100f;
    public float sphereRadius = 6.0f;
    public TargetPopupUI popupUI;

    private RaycastHit lastHit;
    private bool hasHit = false;

    void Update()
    {
        DetectTarget();
    }

    void DetectTarget()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        hasHit = Physics.SphereCast(origin, sphereRadius, direction, out lastHit, detectionRange, detectableLayer);

        if (hasHit)
        {
            GameObject target = lastHit.collider.gameObject;
            TargetData data = target.GetComponent<TargetData>();

            if (data != null)
            {
                string directionText = GetCardinalDirection(transform.eulerAngles.y);
                popupUI.ShowTargetInfo(data.targetId, directionText);
            }
        }
    }

    string GetCardinalDirection(float yaw)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = Mathf.RoundToInt(yaw / 45f) % 8;
        return directions[index];
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, direction * detectionRange);

        // Dibujar el "cono" como dos líneas laterales (aproximación visual)
        Vector3 leftDir = Quaternion.Euler(0, -15f, 0) * direction;
        Vector3 rightDir = Quaternion.Euler(0, 15f, 0) * direction;
        Gizmos.DrawRay(origin, leftDir * detectionRange);
        Gizmos.DrawRay(origin, rightDir * detectionRange);

        // Dibujar esfera en el punto de impacto si hubo detección
        if (hasHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastHit.point, sphereRadius * 0.5f);
        }

        // Dibujar la "tubería" de colisión al final del alcance
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawWireSphere(origin + direction * detectionRange, sphereRadius);
    }
}
