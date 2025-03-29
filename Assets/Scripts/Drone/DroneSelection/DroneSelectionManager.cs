using UnityEngine;

public class DroneSelectionManager : MonoBehaviour
{
    public static DroneSelectionManager Instance { get; private set; }

    public DroneModelInfo SelectedDrone { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedDrone(DroneModelInfo drone)
    {
        SelectedDrone = drone;
    }
}
