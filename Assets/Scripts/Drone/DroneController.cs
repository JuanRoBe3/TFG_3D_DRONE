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
    public float gimbalPitchSpeed = 30f;
    public float gimbalMinAngle = -20f; // mirar un poco hacia arriba
    public float gimbalMaxAngle = 80f;  // mirar bien hacia el suelo

    private float currentGimbalPitch = 0f;

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
        Debug.Log("Pitch: " + gimbalCamera.localRotation.eulerAngles.x);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.right * 2f, Color.red);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.up * 2f, Color.green);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.forward * 2f, Color.blue);

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
        targetRoll = Mathf.Abs(moveX) > 0.01f ? -moveX * maxRollAngle : 0f;

        // Inclinación hacia delante/atrás (pitch)
        targetPitch = Mathf.Abs(moveZ) > 0.01f ? moveZ * maxPitchAngle : 0f;
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

        // Control del pitch de la cámara con teclas F (hacia abajo) y R (hacia arriba)
        float gimbalInput = 0f;
        if (Input.GetKey(KeyCode.F)) gimbalInput = 1f;   // bajar cámara
        if (Input.GetKey(KeyCode.R)) gimbalInput = -1f;  // subir cámara

        currentGimbalPitch += gimbalInput * gimbalPitchSpeed * Time.fixedDeltaTime;
        currentGimbalPitch = Mathf.Clamp(currentGimbalPitch, gimbalMinAngle, gimbalMaxAngle);

        Quaternion localRotation = Quaternion.Euler(currentGimbalPitch, 0f, 0f);
        gimbalCamera.localRotation = localRotation;
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
