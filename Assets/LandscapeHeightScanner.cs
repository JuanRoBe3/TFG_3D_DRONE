using UnityEngine;

public class LandscapeBoundsScanner : MonoBehaviour
{
    public Transform landscapeRoot;

    void Start()
    {
        Debug.Log($"🌍 WorldBounds: {WorldBounds.Value.size}");

        if (landscapeRoot == null)
        {
            Debug.LogError("❌ No has asignado el landscapeRoot.");
            return;
        }

        float maxY = float.MinValue;
        float minY = float.MaxValue;

        Renderer[] renderers = landscapeRoot.GetComponentsInChildren<Renderer>();
        Debug.Log($"🔍 Analizando {renderers.Length} Renderers");

        foreach (Renderer r in renderers)
        {
            float yMin = r.bounds.min.y;
            float yMax = r.bounds.max.y;

            if (yMin < minY) minY = yMin;
            if (yMax > maxY) maxY = yMax;
        }

        Debug.Log($"📦 Altura mínima (por bounds): {minY:F2} m | Altura máxima: {maxY:F2} m");
    }
}
