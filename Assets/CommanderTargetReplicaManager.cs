using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using UnityEngine.UI;

/// <summary>
/// Se encarga de representar visualmente los targets descubiertos,
/// registrar su posición y mostrar la vista ampliada (zoom) al hacer clic.
/// </summary>
public class CommanderTargetReplicaManager : MonoBehaviour
{
    public static CommanderTargetReplicaManager Instance { get; private set; }

    [Header("Prefab del marcador de target")]
    [SerializeField] private GameObject targetReplicaPrefab;

    [Header("🎯 Panel UI que contiene la vista del target")]
    [SerializeField] private CanvasGroup zoomPanel;

    [Header("📺 RawImage donde se muestra la cámara")]
    [SerializeField] private RawImage zoomImage;

    [Header("🎥 Cámara usada para renderizar la vista del target")]
    [SerializeField] private Camera zoomCamera;

    private readonly Dictionary<string, GameObject> targetMarkers = new();
    private readonly ConcurrentQueue<string> payloadQueue = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        MQTTClient.Instance?.RegisterHandler(MQTTConstants.DiscoveredTargetTopic, OnTargetMessageReceived);
    }

    private void OnDisable()
    {
        MQTTClient.Instance?.UnregisterHandler(MQTTConstants.DiscoveredTargetTopic, OnTargetMessageReceived);
    }

    private void OnTargetMessageReceived(string payload)
    {
        payloadQueue.Enqueue(payload);
    }

    private void Update()
    {
        while (payloadQueue.TryDequeue(out var json))
        {
            ProcessPayload(json);
        }
    }

    private void ProcessPayload(string json)
    {
        TargetDiscoveryMessage msg;
        try
        {
            msg = JsonConvert.DeserializeObject<TargetDiscoveryMessage>(json);
        }
        catch
        {
            Debug.LogError("❌ Error al deserializar mensaje de target");
            return;
        }

        if (string.IsNullOrEmpty(msg.targetId))
            return;

        Debug.Log($"📩 Target recibido: {msg.targetId}");

        Vector3 pos = msg.cameraPosition.ToUnityVector3();
        Quaternion rot = msg.cameraRotation.ToUnityQuaternion();

        // Guardar la vista en el sistema centralizado
        TargetDiscoveryReceiver.Instance?.Register(msg.targetId, pos, rot);

        // Evitar duplicados
        if (targetMarkers.ContainsKey(msg.targetId))
            return;

        // Instanciar marcador visual del target
        var marker = Instantiate(targetReplicaPrefab, pos, Quaternion.identity);
        targetMarkers[msg.targetId] = marker;

        // Asignar ID al componente interactivo
        var clickable = marker.GetComponent<ClickableTarget>();
        if (clickable != null)
        {
            clickable.SetTargetId(msg.targetId);
        }
        else
        {
            Debug.LogWarning("⚠️ Prefab del target no tiene ClickableTarget");
        }
    }

    /// <summary>
    /// Muestra el panel de zoom con la vista guardada del target.
    /// </summary>
    public void ShowZoom(string targetId)
    {
        Debug.LogWarning("⚠️ CommanderTargetReplicaManager.ShowZoom() ya no se usa. Usa ZoomedTargetViewPanelManager.ShowTargetView() en su lugar.");
    }

    public void HideZoom()
    {
        Debug.LogWarning("⚠️ CommanderTargetReplicaManager.HideZoom() ya no se usa. Usa ZoomedTargetViewPanelManager.Hide() en su lugar.");
    }

}
