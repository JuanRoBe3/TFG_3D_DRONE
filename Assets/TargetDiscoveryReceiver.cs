using UnityEngine;
using System.Collections.Generic;

public class TargetDiscoveryReceiver : MonoBehaviour
{
    private const string discoveryTopic = MQTTConstants.DiscoveredTargetTopic;

    // Guarda la vista de cada target descubierto
    private Dictionary<string, (Vector3 pos, Quaternion rot)> discoveryViews = new();

    private void OnEnable()
    {
        MQTTClient.Instance.RegisterHandler(discoveryTopic, OnTargetDiscovered);
    }

    private void OnDisable()
    {
        MQTTClient.Instance.UnregisterHandler(discoveryTopic, OnTargetDiscovered);
    }

    private void OnTargetDiscovered(string payload)
    {
        TargetDiscoveryMessage msg = JsonUtility.FromJson<TargetDiscoveryMessage>(payload);

        Debug.Log($"📥 Comandante recibió target {msg.targetId} con vista de cámara.");

        // Guardar vista de cámara
        discoveryViews[msg.targetId] = (msg.cameraPosition.ToUnityVector3(), msg.cameraRotation.ToUnityQuaternion());

        // Buscar el target en escena
        TargetData[] allTargets = FindObjectsOfType<TargetData>();
        foreach (var target in allTargets)
        {
            if (target.targetId == msg.targetId && !target.IsDiscovered)
            {
                target.MarkAsDiscoveredLocally();
                TargetMinimapMarkerManager.Instance?.CreateMarkerForTarget(target);
                break;
            }
        }
    }

    public bool TryGetDiscoveryView(string targetId, out Vector3 pos, out Quaternion rot)
    {
        if (discoveryViews.TryGetValue(targetId, out var view))
        {
            pos = view.pos;
            rot = view.rot;
            return true;
        }

        pos = Vector3.zero;
        rot = Quaternion.identity;
        return false;
    }
}
