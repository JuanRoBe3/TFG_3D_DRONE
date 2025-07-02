using UnityEngine;
using UnityEngine.UI;

public class PilotRuntimeRTAssigner : MonoBehaviour
{
    [Header("RawImages")]
    [SerializeField] private RawImage fpRaw;
    [SerializeField] private RawImage mapRaw;

    [Header("Scene Map Camera")]
    [SerializeField] private Camera mapCam;

    [Header("Opciones")]
    [SerializeField] private int mapRTSize = 1024;
    [SerializeField] private int fpRTSize = 1024;

    private bool fpvAssigned = false;
    private bool dronePending = false;

    void Awake()
    {
        if (!RoleSelection.IsPilot)
        {
            enabled = false;
            return;
        }

        if (!fpRaw)
            fpRaw = GameObject.Find("FPVRawImage")?.GetComponent<RawImage>();

        if (!mapRaw)
            mapRaw = GameObject.Find("MapRawImage")?.GetComponent<RawImage>();

        if (!fpRaw) Debug.LogError("❌ fpRaw no asignado");
        if (!mapRaw) Debug.LogError("❌ mapRaw no asignado");
        if (!mapCam) Debug.LogError("❌ mapCam no asignado");

        if (mapCam && mapRaw && mapRaw.texture == null)
        {
            var mapRT = new RenderTexture(mapRTSize, mapRTSize, 16, RenderTextureFormat.Default);
            mapRT.name = "MapRT";
            mapRT.Create();
            mapCam.targetTexture = mapRT;
            mapRaw.texture = mapRT;
            Debug.Log("🗺️ RenderTexture para el minimapa creada y asignada");
        }

        DroneLoader.OnDroneInstantiated += OnDrone;

        // Marcar si hay que buscar el dron más adelante
        if (GameObject.FindWithTag("Drone") == null)
            dronePending = true;
    }

    void Start()
    {
        if (dronePending && !fpvAssigned)
        {
            GameObject drone = GameObject.FindWithTag("Drone");
            if (drone)
            {
                Debug.Log("🔁 Dron detectado en Start(), llamando a OnDrone");
                OnDrone(drone);
            }
        }
    }

    void OnDestroy()
    {
        DroneLoader.OnDroneInstantiated -= OnDrone;
    }

    void OnDrone(GameObject drone)
    {
        if (fpvAssigned) return;
        if (!fpRaw) { Debug.LogError("❌ No se puede asignar FPV, fpRaw es null"); return; }

        Transform pilotRoot = drone.transform.Find("PilotCamera");
        if (!pilotRoot)
        {
            Debug.LogError("❌ No se encontró 'PilotCamera' como hijo del dron instanciado");
            return;
        }

        Camera fpCam = pilotRoot.GetComponentInChildren<Camera>(false);
        if (!fpCam)
        {
            Debug.LogError("❌ No se encontró cámara activa dentro de 'PilotCamera'");
            return;
        }

        RenderTexture fpRT = new RenderTexture(fpRTSize, fpRTSize, 24, RenderTextureFormat.Default);
        fpRT.name = "FPVRT";
        fpRT.Create();

        fpCam.targetTexture = fpRT;
        fpRaw.texture = fpRT;
        fpvAssigned = true;

        Debug.Log($"✅ FPV RenderTexture asignada correctamente ({fpRT.width}x{fpRT.height}) a '{fpCam.name}' y RawImage '{fpRaw.name}'");

        var pub = FindObjectOfType<DroneCameraPublisher>();
        if (pub)
        {
            pub.SetCamera(fpCam.transform);  // ✅ corregido
        }
    }
}
