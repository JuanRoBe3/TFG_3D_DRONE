using UnityEngine;
using UnityEngine.UI;

public class PilotRuntimeRTAssigner : MonoBehaviour
{
    [Header("RawImages")]
    [SerializeField] RawImage fpRaw;
    [SerializeField] RawImage mapRaw;

    [Header("Scene Map Camera")]
    [SerializeField] Camera mapCam;   // arrastra SceneMapTopDownCamera aquí

    void Awake()
    {
        if (!RoleSelection.IsPilot)
        {
            enabled = false;
            return;
        }

        Debug.Log("🎮 [PilotRuntimeRTAssigner] Activado como piloto");

        // Crear RenderTexture para el minimapa si no está asignada
        if (mapCam.targetTexture == null || mapRaw.texture == null)
        {
            Debug.Log("🖼️ Creando nueva RenderTexture para minimapa...");
            RenderTexture mapRT = RTFactory.New(512, 512, 16);
            mapCam.targetTexture = mapRT;
            mapRaw.texture = mapRT;
        }
        else
        {
            Debug.Log("🧠 Ya hay RenderTexture asignada al minimapa");
        }

        // Suscribirse al evento de dron instanciado
        DroneLoader.OnDroneInstantiated += OnDrone;

        // Por si el evento ya ocurrió antes de que se activara este script
        var drone = GameObject.FindWithTag("Drone");
        if (drone) OnDrone(drone);
    }

    void OnDestroy()
    {
        DroneLoader.OnDroneInstantiated -= OnDrone;
    }

    void OnDrone(GameObject drone)
    {
        Debug.Log("🛰️ OnDrone recibido: " + drone.name);

        // Buscar el objeto contenedor llamado "PilotCamera"
        Transform pilotRoot = drone.transform.Find("PilotCamera");
        if (!pilotRoot)
        {
            Debug.LogError("❌ No se encontró el objeto PilotCamera");
            return;
        }

        // Buscar una cámara ACTIVA dentro de ese objeto
        Camera fpCam = pilotRoot.GetComponentInChildren<Camera>(false);  // ⬅️ solo activa
        if (!fpCam)
        {
            Debug.LogError("❌ No se encontró ninguna cámara ACTIVA dentro de PilotCamera");
            return;
        }

        Debug.Log($"📷 Cámara FPV encontrada: {fpCam.name}");

        // Crear RenderTexture y asignarla a la cámara y RawImage
        RenderTexture fpRT = RTFactory.New();  // por defecto: 1024×1024×24
        fpCam.targetTexture = fpRT;
        fpRaw.texture = fpRT;
        Debug.Log($"✅ RenderTexture asignada a fpCam '{fpCam.name}' y RawImage '{fpRaw.name}'");

        // Asignar referencias a DroneCameraPublisher
        var pub = FindObjectOfType<DroneCameraPublisher>();
        if (pub)
        {
            pub.SetCameras(fpCam.transform, mapCam.transform);
            Debug.Log("📡 Cámaras asignadas al DroneCameraPublisher");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró DroneCameraPublisher");
        }
    }
}
