using UnityEngine;
using TMPro;

public class CommanderDroneInfoPanel : MonoBehaviour
{
    public static CommanderDroneInfoPanel Instance { get; private set; }

    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textSignal;
    [SerializeField] private TextMeshProUGUI textAltitude;
    [SerializeField] private TextMeshProUGUI textAGL;

    private Transform droneTransform;
    private DroneData droneData;
    private float terrainY;
    private bool active = false;

    private void Awake()
    {
        Instance = this;
        ClearPanel(); // inicializa limpio
    }

    private void Update()
    {
        if (!active || droneTransform == null) return;
        UpdateUI();
    }

    public void SetDrone(Transform droneTransform, DroneData droneData)
    {
        this.droneTransform = droneTransform;
        this.droneData = droneData;
        this.terrainY = GetTerrainHeightBelow(droneTransform.position);
        this.active = true;

        textName.text = droneData.droneName;
        textSignal.text = "Signal OK"; // Esto puede venir por MQTT si tienes un sistema real
    }

    public void ClearPanel()
    {
        active = false;
        textName.text = "—";
        textSignal.text = "—";
        textAltitude.text = "—";
        textAGL.text = "—";
    }

    private void UpdateUI()
    {
        float height = droneTransform.position.y;
        float agl = height - terrainY;

        textAltitude.text = $"{height:F1} m";
        textAGL.text = $"AGL {agl:F1} m";
    }

    private float GetTerrainHeightBelow(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 1000f))
        {
            return hit.point.y;
        }
        return 0f;
    }
}
