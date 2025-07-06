using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public LayerMask obstacleLayer;

    [Header("HUD Warning Manager")]
    public DroneHUDWarningManager hudWarningManager;

    [Header("UI Reference")]
    public CollisionDistanceUI distanceUI;

    [Header("Visual Warning Manager")]
    public DroneVisualWarningManager visualWarningManager;

    private static readonly Vector3[] directions = new Vector3[]
    {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
        Vector3.up, Vector3.down,
        (Vector3.forward + Vector3.left).normalized,
        (Vector3.forward + Vector3.right).normalized,
        (Vector3.back + Vector3.left).normalized,
        (Vector3.back + Vector3.right).normalized,
        (Vector3.up + Vector3.forward).normalized,
        (Vector3.up + Vector3.back).normalized,
        (Vector3.up + Vector3.left).normalized,
        (Vector3.up + Vector3.right).normalized,
        (Vector3.down + Vector3.forward).normalized,
        (Vector3.down + Vector3.back).normalized,
        (Vector3.down + Vector3.left).normalized,
        (Vector3.down + Vector3.right).normalized,
        (Vector3.up + Vector3.forward + Vector3.left).normalized,
        (Vector3.up + Vector3.forward + Vector3.right).normalized,
        (Vector3.up + Vector3.back + Vector3.left).normalized,
        (Vector3.up + Vector3.back + Vector3.right).normalized,
        (Vector3.down + Vector3.forward + Vector3.left).normalized,
        (Vector3.down + Vector3.forward + Vector3.right).normalized,
        (Vector3.down + Vector3.back + Vector3.left).normalized,
        (Vector3.down + Vector3.back + Vector3.right).normalized
    };

    void Update()
    {
        float? leftDistance = null;
        float? rightDistance = null;
        float? backFanDistance = null; // ← nuevo sistema de colisión trasera

        distanceUI?.ClearLeftDistance();
        distanceUI?.ClearRightDistance();
        distanceUI?.ClearBackDistance();

        // 👉 Análisis con múltiples rayos generales (solo para left y right)
        foreach (var dir in directions)
        {
            Vector3 worldDir = transform.TransformDirection(dir);

            if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, detectionRadius, obstacleLayer))
            {
                float dotLeft = Vector3.Dot(worldDir, transform.TransformDirection(Vector3.left));
                float dotRight = Vector3.Dot(worldDir, transform.TransformDirection(Vector3.right));

                if (dotLeft > 0.7f && (leftDistance == null || hit.distance < leftDistance.Value))
                    leftDistance = hit.distance;

                if (dotRight > 0.7f && (rightDistance == null || hit.distance < rightDistance.Value))
                    rightDistance = hit.distance;

                Debug.DrawRay(transform.position, worldDir * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, worldDir * detectionRadius, Color.gray);
            }
        }

        // 🔙 Haz trasero: abanico de rayos hacia atrás
        Vector3[] backRays = GetBackFanDirections(7, 30f); // 7 rayos, 30° spread

        foreach (var ray in backRays)
        {
            if (Physics.Raycast(transform.position, ray, out RaycastHit backHit, detectionRadius, obstacleLayer))
            {
                if (backFanDistance == null || backHit.distance < backFanDistance.Value)
                    backFanDistance = backHit.distance;

                Debug.DrawRay(transform.position, ray * backHit.distance, Color.magenta);
            }
            else
            {
                Debug.DrawRay(transform.position, ray * detectionRadius, Color.gray);
            }
        }

        // 🖥️ Mostrar distancias
        if (leftDistance != null) distanceUI?.UpdateLeftDistance(leftDistance.Value);
        if (rightDistance != null) distanceUI?.UpdateRightDistance(rightDistance.Value);
        if (backFanDistance != null) distanceUI?.UpdateBackDistance(backFanDistance.Value);

        // 🔴 Intensidades HUD
        float leftIntensity = leftDistance.HasValue ? 1f - Mathf.Clamp01(leftDistance.Value / detectionRadius) : 0f;
        float rightIntensity = rightDistance.HasValue ? 1f - Mathf.Clamp01(rightDistance.Value / detectionRadius) : 0f;
        float backIntensity = backFanDistance.HasValue ? 1f - Mathf.Clamp01(backFanDistance.Value / detectionRadius) : 0f;

        hudWarningManager?.UpdateWarnings(leftIntensity, rightIntensity, backIntensity);
        // 🔴 Elementos visuales sobre el dron
        visualWarningManager?.UpdateVisuals(leftIntensity, rightIntensity, backIntensity);

    }

    private Vector3[] GetBackFanDirections(int rayCount, float spreadAngle)
    {
        Vector3[] rays = new Vector3[rayCount];

        for (int i = 0; i < rayCount; i++)
        {
            float angle = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, i / (rayCount - 1f));
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.back;
            rays[i] = transform.TransformDirection(dir.normalized);
        }

        return rays;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Usa orientación del dron
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 back = transform.TransformDirection(Vector3.back);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + forward * detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + back * detectionRadius);
    }
}
