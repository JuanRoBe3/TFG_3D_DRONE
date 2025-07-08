using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// Se encarga de manejar el descubrimiento de targets:
/// - Marca el estado interno como descubierto.
/// - Genera el marcador visual en escena.
/// - Publica la información por MQTT al comandante.
/// </summary>
public class TargetDiscoveryManager : MonoBehaviour
{
    public static TargetDiscoveryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Gestiona el descubrimiento de un target desde el piloto.
    /// </summary>
    public async void HandleTargetDiscovered(TargetData target, Transform discoveryCamera)
    {
        if (target == null || target.IsDiscovered)
        {
            Debug.Log("🔁 El target ya fue descubierto o es nulo.");
            return;
        }

        // 1️⃣ Marca como descubierto (estado local)
        target.MarkAsDiscoveredLocally();

        // 2️⃣ Crear marcador visual (pin)
        TargetMinimapMarkerManager.Instance?.CreateMarkerForTarget(target);

        // 3️⃣ Publicar por MQTT si tenemos cámara
        if (discoveryCamera != null)
        {
            await TargetDiscoveryPublisher.Instance.PublishDiscoveredTarget(
                target.targetId,
                discoveryCamera.position,
                discoveryCamera.rotation
            );
        }
    }
}
