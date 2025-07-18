using UnityEngine;
using UnityEngine.InputSystem; // NUEVO
using UnityEngine.InputSystem.Controls;

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
    public float gimbalMinAngle = -20f;
    public float gimbalMaxAngle = 80f;

    private float currentGimbalPitch = 0f;

    //private float positionUpdateTimer = 0f; //NO SIRVE YA CREO
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
        //UpdateGimbalCamera();
    }

    private void Update()
    {
        //HandlePositionUpdate();
        Debug.Log("Pitch: " + gimbalCamera.localRotation.eulerAngles.x);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.right * 2f, Color.red);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.up * 2f, Color.green);
        Debug.DrawRay(gimbalCamera.position, gimbalCamera.forward * 2f, Color.blue);
    }

    private void HandleMovement()
    {
        var joystick = Joystick.current;

        Vector2 stick = joystick != null ? joystick.stick.ReadValue() : Vector2.zero;
        float twist = joystick != null ? joystick.twist.ReadValue() : 0f;

        float joystickVertical = 0f;
        if (joystick != null)
        {
            if (joystick.TryGetChildControl<ButtonControl>("button11")?.isPressed == true)
                joystickVertical = 1f;
            if (joystick.TryGetChildControl<ButtonControl>("button13")?.isPressed == true)
                joystickVertical = -1f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;
        if (Input.GetKey(KeyCode.Space)) moveY = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) moveY = -1f;

        float finalX = moveX + stick.x;
        float finalZ = moveZ + stick.y;
        float finalY = moveY + joystickVertical;

        Vector3 input = new Vector3(finalX, finalY * (verticalSpeed / speed), finalZ);
        Vector3 moveDirection = transform.TransformDirection(input.normalized) * speed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.Q)) yawInput = -1f;
        if (Input.GetKey(KeyCode.E)) yawInput = 1f;
        yawInput += twist;

        transform.Rotate(Vector3.up * yawInput * yawSpeed * Time.fixedDeltaTime, Space.Self);

        targetRoll = Mathf.Abs(finalX) > 0.01f ? -finalX * maxRollAngle : 0f;
        targetPitch = Mathf.Abs(finalZ) > 0.01f ? finalZ * maxPitchAngle : 0f;
    }

    private void HandleVisualTilt()
    {
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * tiltSmoothSpeed);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * tiltSmoothSpeed);

        Quaternion tilt = Quaternion.Euler(currentPitch, transform.rotation.eulerAngles.y, currentRoll);
        transform.rotation = tilt;
    }

    /* //NOT WANTED ANYMORE THE ROTATION OF THE CAMERA
    private void UpdateGimbalCamera()
    {
        if (gimbalCamera == null) return;

        float gimbalInput = 0f;

        // 🎮 Control con botones del joystick (13 = abajo, 11 = arriba)
        var joystick = Joystick.current;
        if (joystick != null)
        {
            if (joystick.TryGetChildControl<ButtonControl>("button5")?.isPressed == true)
                gimbalInput = 1f; // mirar hacia abajo
            if (joystick.TryGetChildControl<ButtonControl>("button6")?.isPressed == true)
                gimbalInput = -1f; // mirar hacia arriba
        }

        // ⌨️ Control alternativo (F / R)
        if (Input.GetKey(KeyCode.F)) gimbalInput = 1f;
        if (Input.GetKey(KeyCode.R)) gimbalInput = -1f;

        currentGimbalPitch += gimbalInput * gimbalPitchSpeed * Time.fixedDeltaTime;
        currentGimbalPitch = Mathf.Clamp(currentGimbalPitch, gimbalMinAngle, gimbalMaxAngle);

        Quaternion localRotation = Quaternion.Euler(currentGimbalPitch, 0f, 0f);
        gimbalCamera.localRotation = localRotation;
    }
    */

    // CREO QUE YA NO SIRVE LA VERDAD
    /*
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
            //publisher.PublishMessage(MQTTConstants.DroneCameraTopic, message);
        }
    }
    */
}
