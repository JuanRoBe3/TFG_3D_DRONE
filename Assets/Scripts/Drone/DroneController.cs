using UnityEngine;

public class DroneController : MonoBehaviour
{
    private MQTTPublisher publisher;
    public float speed = DroneConstants.DefaultSpeed; // Uses drone movement speed constant
    private float positionUpdateTimer = 0f; // Timer for position updates

    private void Start()
    {
        if (MQTTClient.Instance == null)
        {
            Debug.LogError(LogMessagesConstants.ErrorMQTTClientNotFound);
        }
        else
        {
            publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
        }
    }

    private void Update()
    {
        HandleMovement();
        HandlePositionUpdate();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // Movement along the X-axis (A/D or Arrow keys)
        float moveZ = Input.GetAxis("Vertical");   // Movement along the Z-axis (W/S or Arrow keys)
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Space)) moveY = 1f;  // Ascend with SPACE key
        if (Input.GetKey(KeyCode.LeftShift)) moveY = -1f; // Descend with SHIFT key

        Vector3 move = new Vector3(moveX, moveY, moveZ) * speed * Time.deltaTime;
        transform.position += move;
    }

    private void HandlePositionUpdate()
    {
        positionUpdateTimer += Time.deltaTime;
        if (positionUpdateTimer >= DroneConstants.PositionUpdateInterval && MQTTClient.Instance != null)
        {
            SendPosition();
            positionUpdateTimer = 0f; // Reset the timer
        }
    }

    private void SendPosition()
    {
        if (MQTTClient.Instance != null)
        {
            Debug.Log("fenooooomeno, instance no es null");
            Vector3 position = transform.position;
            string message = $"{{\"x\": {position.x}, \"y\": {position.y}, \"z\": {position.z}}}";
            publisher.PublishMessage(MQTTConstants.DronePositionTopic, message);
        }
    }
}
