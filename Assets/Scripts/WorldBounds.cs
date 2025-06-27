using UnityEngine;

/// <summary>Calcula y expone los bounds mundiales del landscape.</summary>
public sealed class WorldBounds : MonoBehaviour
{
    public static Bounds Value { get; private set; }

    [Tooltip("Objeto padre que agrupa todos los meshes del paisaje")]
    [SerializeField] private Transform landscapeRoot;

    void Awake()
    {
        if (landscapeRoot == null)
        {
            Debug.LogError("WorldBounds: landscapeRoot sin asignar");
            return;
        }

        Renderer[] rs = landscapeRoot.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0)
        {
            Debug.LogError("WorldBounds: no hay Renderers bajo landscapeRoot");
            return;
        }

        Bounds b = new Bounds(rs[0].bounds.center, Vector3.zero);
        foreach (Renderer r in rs) b.Encapsulate(r.bounds);

        Value = b;
        Debug.Log($"🌍 World bounds -> W:{b.size.x:F1}  D:{b.size.z:F1}  H:{b.size.y:F1}");
    }
}
