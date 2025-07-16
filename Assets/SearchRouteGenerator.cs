using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Busca todas las zonas de búsqueda activas y genera rutas zigzag con objetos 3D (prefabs).
/// </summary>
public class SearchRouteGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject routePointPrefab;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Configuración")]
    [SerializeField] private float margin = 50f;
    [SerializeField] private float spacingBetweenPasses = 150f;
    [SerializeField] private float spacingBetweenPoints = 50f;

    public void GenerateRoutesForAllZones()
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag("SearchZone");

        foreach (GameObject zone in zones)
        {
            GenerateRouteForZone(zone);
        }
    }

    public void GenerateRouteForZone(GameObject searchZone)
    {
        Renderer[] renderers = searchZone.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"❌ SearchZone '{searchZone.name}' no tiene Renderers en hijos.");
            return;
        }

        // Calcular bounds combinados
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        float usableWidth = bounds.size.x - 2 * margin;
        float usableHeight = bounds.size.z - 2 * margin;

        Vector3 bottomLeft = new Vector3(bounds.min.x + margin, bounds.center.y, bounds.min.z + margin);
        int numPasses = Mathf.FloorToInt(usableWidth / spacingBetweenPasses);
        int numPointsPerPass = Mathf.CeilToInt(usableHeight / spacingBetweenPoints);

        List<Vector3> pathPoints = new();

        for (int i = 0; i <= numPasses; i++)
        {
            float offsetX = i * spacingBetweenPasses;
            Vector3 start = bottomLeft + new Vector3(offsetX, 0, 0);
            Vector3 end = start + new Vector3(0, 0, usableHeight);

            // Trayectoria hacia adelante o hacia atrás
            if (i % 2 == 0)
            {
                for (int j = 0; j <= numPointsPerPass; j++)
                {
                    float t = j / (float)numPointsPerPass;
                    Vector3 point = Vector3.Lerp(start, end, t);
                    pathPoints.Add(point);
                }
            }
            else
            {
                for (int j = 0; j <= numPointsPerPass; j++)
                {
                    float t = j / (float)numPointsPerPass;
                    Vector3 point = Vector3.Lerp(end, start, t);
                    pathPoints.Add(point);
                }
            }

            // Agregar punto de giro al final de cada pasada, excepto la última
            if (i < numPasses)
            {
                Vector3 lastPoint = pathPoints[^1]; // Último punto añadido
                Vector3 offsetTurn = new Vector3(spacingBetweenPasses / 2f, 0, 0); // Pequeño desplazamiento para simular giro

                // Si iba hacia adelante, el giro será hacia +X, luego +Z o -Z
                if (i % 2 == 0)
                {
                    pathPoints.Add(lastPoint + offsetTurn);
                }
                else
                {
                    pathPoints.Add(lastPoint + offsetTurn);
                }
            }
        }

        InstantiatePoints(pathPoints);
        InstantiateArrow(pathPoints);
    }

    private void InstantiatePoints(List<Vector3> points)
    {
        foreach (Vector3 pos in points)
        {
            Instantiate(routePointPrefab, pos, Quaternion.identity);
        }
    }

    private void InstantiateArrow(List<Vector3> points)
    {
        if (arrowPrefab == null || points.Count < 2)
            return;

        Vector3 end = points[^1];
        Vector3 prev = points[^2];
        Vector3 dir = (end - prev).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        Instantiate(arrowPrefab, end, rot);
    }


}
