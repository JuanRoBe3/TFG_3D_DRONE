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
        float? backFanDistance = null;
        float? belowDistance = null; // 👈 nuevo

        distanceUI?.ClearLeftDistance();
        distanceUI?.ClearRightDistance();
        distanceUI?.ClearBackDistance();

        // 🔍 Raycasts generales (solo afectan a left, right y abajo)
        foreach (var dir in directions)
        {
            Vector3 worldDir = transform.TransformDirection(dir);

            if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, detectionRadius, obstacleLayer))
            {
                float dotLeft = Vector3.Dot(worldDir, transform.TransformDirection(Vector3.left));
                float dotRight = Vector3.Dot(worldDir, transform.TransformDirection(Vector3.right));
                float dotDown = Vector3.Dot(worldDir, transform.TransformDirection(Vector3.down));

                if (dotLeft > 0.7f && (leftDistance == null || hit.distance < leftDistance.Value))
                    leftDistance = hit.distance;

                if (dotRight > 0.7f && (rightDistance == null || hit.distance < rightDistance.Value))
                    rightDistance = hit.distance;

                if (dotDown > 0.7f && (belowDistance == null || hit.distance < belowDistance.Value))
                    belowDistance = hit.distance;

                Debug.DrawRay(transform.position, worldDir * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, worldDir * detectionRadius, Color.gray);
            }
        }

        // 🔙 Haz trasero (solo back)
        Vector3[] backRays = GetBackFanDirections(7, 30f);

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

        // 🖥️ UI de distancias (solo si hay datos)
        if (leftDistance != null) distanceUI?.UpdateLeftDistance(leftDistance.Value);
        if (rightDistance != null) distanceUI?.UpdateRightDistance(rightDistance.Value);

        // ⬅️ ➡️ Intensidades izquierda y derecha
        float leftIntensity = leftDistance.HasValue ? 1f - Mathf.Clamp01(leftDistance.Value / detectionRadius) : 0f;
        float rightIntensity = rightDistance.HasValue ? 1f - Mathf.Clamp01(rightDistance.Value / detectionRadius) : 0f;

        // 🔽 COMBINAR trasero + inferior en una sola lógica
        float? combinedBackDistance = null;
        if (backFanDistance.HasValue && belowDistance.HasValue)
            combinedBackDistance = Mathf.Min(backFanDistance.Value, belowDistance.Value);
        else if (backFanDistance.HasValue)
            combinedBackDistance = backFanDistance.Value;
        else if (belowDistance.HasValue)
            combinedBackDistance = belowDistance.Value;

        float backIntensity = combinedBackDistance.HasValue
            ? 1f - Mathf.Clamp01(combinedBackDistance.Value / detectionRadius)
            : 0f;

        if (combinedBackDistance.HasValue)
            distanceUI?.UpdateBackDistance(combinedBackDistance.Value);

        // 📡 HUD y visuales del dron
        hudWarningManager?.UpdateWarnings(leftIntensity, rightIntensity, backIntensity);
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
