using UnityEngine;

// USED FOR THE DUMMY DRONES

public class DroneReplicaInitializer : MonoBehaviour
{
    [SerializeField] private string droneId = "Dummy1";
    [SerializeField] private Camera fpvCam;

    void Awake()
    {
        if (fpvCam == null) fpvCam = GetComponentInChildren<Camera>();

        var rt = RenderTextureRegistry.GetOrCreate(droneId);  // mismo factory
        fpvCam.targetTexture = rt;

        DroneViewPanelManager.Register(droneId, rt);
        GetComponent<ClickableDrone>().SetId(droneId);
    }
}
