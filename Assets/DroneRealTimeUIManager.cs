using UnityEngine;
using TMPro;

public class DroneRealTimeUIManager : MonoBehaviour
{
    public static DroneRealTimeUIManager Instance { get; private set; }

    [Header("Referencias a los textos")]
    [SerializeField] private TextMeshProUGUI textDuration;
    [SerializeField] private TextMeshProUGUI textUsedData;
    [SerializeField] private TextMeshProUGUI textDistance;
    [SerializeField] private TextMeshProUGUI textHeight;

    private Transform droneTransform;
    private DroneData droneData;

    private float startTime;
    private float usedDataMB;

    [Header("Tasa de consumo de datos (MB/segundo)")]
    [SerializeField] private float dataUsageRate = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicialización segura si no hay datos aún
        if (textDistance != null)
            textDistance.text = "—";
    }

    private void Update()
    {
        if (droneTransform == null || droneData == null) return;

        UpdateDuration();
        UpdateUsedData();
        UpdateHeight();
    }

    private void UpdateDuration()
    {
        float elapsed = Time.time - startTime;
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);
        textDuration.text = $"{minutes:D2}:{seconds:D2}";
    }

    private void UpdateUsedData()
    {
        usedDataMB += dataUsageRate * Time.deltaTime;
        float clampedUsed = Mathf.Min(usedDataMB, droneData.storageCapacityMB);
        textUsedData.text = $"{clampedUsed:F1} / {droneData.storageCapacityMB} MB";
    }

    private void UpdateHeight()
    {
        float height = droneTransform.position.y;
        textHeight.text = $"{height:F1} m";
    }

    /// <summary>
    /// Método que se llama al instanciar el dron.
    /// </summary>
    public void SetDroneReference(Transform droneTransform, DroneData droneData)
    {
        this.droneTransform = droneTransform;
        this.droneData = droneData;
        this.usedDataMB = 0f;
        this.startTime = Time.time;
    }
}
