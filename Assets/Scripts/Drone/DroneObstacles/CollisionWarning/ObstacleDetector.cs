using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public LayerMask obstacleLayer;

    [Header("HUD Reference")]
    public DroneHUDWarning hudWarning;

    [Header("UI Reference")]
    public CollisionDistanceUI distanceUI; // ← UI para mostrar en pantalla izquierda/derecha

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
        float minDistance = float.MaxValue;
        bool obstacleDetected = false;

        float? leftDistance = null;
        float? rightDistance = null;

        // Limpiar los textos de la UI al principio del frame
        distanceUI?.ClearLeftDistance();
        distanceUI?.ClearRightDistance();

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, detectionRadius, obstacleLayer))
            {
                obstacleDetected = true;

                if (hit.distance < minDistance)
                    minDistance = hit.distance;

                if (Vector3.Dot(dir, Vector3.left) > 0.7f)
                {
                    if (leftDistance == null || hit.distance < leftDistance.Value)
                        leftDistance = hit.distance;
                }

                if (Vector3.Dot(dir, Vector3.right) > 0.7f)
                {
                    if (rightDistance == null || hit.distance < rightDistance.Value)
                        rightDistance = hit.distance;
                }

                Debug.DrawRay(transform.position, dir * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, dir * detectionRadius, Color.gray);
            }
        }

        if (obstacleDetected)
        {
            float intensity = 1f - Mathf.Clamp01(minDistance / detectionRadius);
            hudWarning?.SetWarningIntensity(intensity);

            if (leftDistance != null)
                distanceUI?.UpdateLeftDistance(leftDistance.Value);

            if (rightDistance != null)
                distanceUI?.UpdateRightDistance(rightDistance.Value);
        }
        else
        {
            hudWarning?.SetWarningIntensity(0f);
        }
    }
}
