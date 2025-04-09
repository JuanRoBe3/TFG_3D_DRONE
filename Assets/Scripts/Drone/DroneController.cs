using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    private MQTTPublisher publisher;
    public float speed = DroneConstants.DefaultSpeed;
    public float verticalSpeed = 3f;
    public float yawSpeed = 90f;

    [Header("Visual Tilt Settings")]
    public float maxPitchAngle = 10f;
    public float maxRollAngle = 15f;
    public float tiltSmoothSpeed = 5f;

    [Header("Gimbal Camera")]
    public Transform gimbalCamera;

    private float positionUpdateTimer = 0f;
    private Rigidbody rb;

    private float currentPitch = 0f;
    private float currentRoll = 0f;
    private float targetPitch = 0f;
    private float targetRoll = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (MQTTClient.Instance == null)
        {
            Debug.LogError(LogMessagesConstants.ErrorMQTTClientNotFound);
        }
        else
        {
            publisher = new MQTTPublisher(MQTTClient.Instance.GetClient());
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleVisualTilt();
        UpdateGimbalCamera();
    }

    private void Update()
    {
        HandlePositionUpdate();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Space)) moveY = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) moveY = -1f;

        Vector3 input = new Vector3(moveX, moveY * (verticalSpeed / speed), moveZ);
        Vector3 moveDirection = transform.TransformDirection(input.normalized) * speed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;
        transform.Rotate(Vector3.up * yawInput * yawSpeed * Time.fixedDeltaTime, Space.Self);

        // Inclinación lateral (roll)
        if (Mathf.Abs(moveX) > 0.01f)
            targetRoll = -moveX * maxRollAngle;
        else
            targetRoll = 0f;

        // ✅ Inclinación hacia delante/atrás (pitch) corregida
        if (Mathf.Abs(moveZ) > 0.01f)
            targetPitch = moveZ * maxPitchAngle; // corregido aquí
        else
            targetPitch = 0f;
    }

    private void HandleVisualTilt()
    {
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * tiltSmoothSpeed);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * tiltSmoothSpeed);

        Quaternion tilt = Quaternion.Euler(currentPitch, transform.rotation.eulerAngles.y, currentRoll);
        transform.rotation = tilt;
    }

    private void UpdateGimbalCamera()
    {
        if (gimbalCamera == null) return;

        Vector3 cameraEuler = gimbalCamera.rotation.eulerAngles;
        cameraEuler.x = 0f;
        cameraEuler.z = 0f;
        gimbalCamera.rotation = Quaternion.Euler(cameraEuler);
    }

    private void HandlePositionUpdate()
    {
        positionUpdateTimer += Time.deltaTime;
        if (positionUpdateTimer >= DroneConstants.PositionUpdateInterval && MQTTClient.Instance != null)
        {
            SendPosition();
            positionUpdateTimer = 0f;
        }
    }

    private void SendPosition()
    {
        if (MQTTClient.Instance != null)
        {
            Vector3 position = transform.position;
            string message = $"{{\"x\": {position.x}, \"y\": {position.y}, \"z\": {position.z}}}";
            publisher.PublishMessage(MQTTConstants.DronePositionTopic, message);
        }
    }
}
