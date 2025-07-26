using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PilotRouteProgressManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tag que tiene el dron instanciado en runtime")]
    private string droneTag = "Drone";

    [Tooltip("Slider de la UI que representa el progreso de ruta")]
    [SerializeField] private Slider progressBar;

    [Tooltip("Distancia máxima para considerar un punto recorrido")]
    private float reachThreshold = 150f; // This should be approximately the same as the "Spacing Between Passes" in the SearchRouteGeneratos for being realistic. In this case 150

    private Transform droneTransform;
    private List<RoutePoint> routePoints = new();

    void Start()
    {
        RefreshRoutePoints();
    }

    void Update()
    {
        // Buscar el dron si aún no se ha detectado
        if (droneTransform == null)
        {
            GameObject drone = GameObject.FindGameObjectWithTag(droneTag);
            if (drone != null)
            {
                droneTransform = drone.transform;
                Debug.Log("✅ Dron encontrado por tag.");
            }
            else
            {
                return;
            }
        }

        // Si aún no hay puntos (porque se generaron tarde), no hacemos nada
        if (routePoints.Count == 0) return;

        int visitedCount = 0;

        foreach (var point in routePoints)
        {
            if (!point.visited)
            {
                Vector3 droneXZ = new Vector3(droneTransform.position.x, 0, droneTransform.position.z);
                Vector3 pointXZ = new Vector3(point.transform.position.x, 0, point.transform.position.z);

                float distance = Vector3.Distance(droneXZ, pointXZ);
                if (distance < reachThreshold)
                {
                    point.visited = true;
                    Debug.Log($"✅ Punto visitado: {point.name} a {distance:F2} unidades");
                }
            }

            if (point.visited) visitedCount++;
        }

        progressBar.value = visitedCount;
    }

    /// <summary>
    /// Refresca la lista de puntos de ruta encontrados en la escena.
    /// Llamar cuando se generen dinámicamente.
    /// </summary>
    public void RefreshRoutePoints()
    {
        routePoints.Clear();
        routePoints.AddRange(FindObjectsOfType<RoutePoint>());
        Debug.Log($"🔄 Puntos de ruta detectados: {routePoints.Count}");

        progressBar.maxValue = routePoints.Count;
        progressBar.value = 0;
        progressBar.gameObject.SetActive(routePoints.Count > 0);
    }

    [ContextMenu("🧪 Force Refresh RoutePoints")]
    private void ForceRefresh()
    {
        RefreshRoutePoints();
    }
}
