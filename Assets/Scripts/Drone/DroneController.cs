using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;

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
    private Rigidbody rb;

    private float currentPitch = 0f;
    private float currentRoll = 0f;
    private float targetPitch = 0f;
    private float targetRoll = 0f;

    // 🟩 ESTABILIZACIÓN HACIA ESTADO ANTERIOR (rebote suave forzado)
    private bool isStabilizing = false;
    private float stabilizeTimer = 0f;
    private float stabilizeDuration = 2f;
    private Vector3 reboundTargetPosition;
    private Quaternion reboundTargetRotation;
    private Quaternion reboundStartRotation;
    private Vector3 reboundStartPosition;

    // 🧠 HISTORIAL DE ESTADOS (para recuperación)
    private class DroneState
    {
        public Vector3 position;
        public Quaternion rotation;

        public DroneState(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    private Queue<DroneState> stateHistory = new Queue<DroneState>();
    private float stateRecordInterval = 0.1f;
    private float timeSinceLastRecord = 0f;
    private float stateMemoryDuration = 2f; // segundos hacia atrás

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
        if (isStabilizing)
        {
            stabilizeTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(stabilizeTimer / stabilizeDuration);

            // ✅ Interpolación suave entre estado actual y estado de hace 2s
            Vector3 interpolatedPosition = Vector3.Lerp(reboundStartPosition, reboundTargetPosition, t);
            Quaternion interpolatedRotation = Quaternion.Slerp(reboundStartRotation, reboundTargetRotation, t);

            rb.MovePosition(interpolatedPosition);
            rb.MoveRotation(interpolatedRotation);

            if (stabilizeTimer >= stabilizeDuration)
            {
                isStabilizing = false;
            }
        }

        HandleMovement();
        HandleVisualTilt();

        // 🧠 Guardar estado en historial cada intervalo
        timeSinceLastRecord += Time.fixedDeltaTime;
        if (timeSinceLastRecord >= stateRecordInterval)
        {
            stateHistory.Enqueue(new DroneState(rb.position, rb.rotation));
            timeSinceLastRecord = 0f;

            while (stateHistory.Count > stateMemoryDuration / stateRecordInterval)
            {
                stateHistory.Dequeue();
            }
        }
    }

    private void Update()
    {
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
        if (isStabilizing) return;

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * tiltSmoothSpeed);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * tiltSmoothSpeed);

        Quaternion tilt = Quaternion.Euler(currentPitch, transform.rotation.eulerAngles.y, currentRoll);
        transform.rotation = tilt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isStabilizing && stateHistory.Count > 0)
        {
            isStabilizing = true;
            stabilizeTimer = 0f;

            // 🔁 Estado objetivo al que volver (≈ 2s atrás)
            DroneState reboundState = stateHistory.Peek();
            reboundTargetPosition = reboundState.position;
            reboundTargetRotation = reboundState.rotation;

            // 🔁 Estado actual de partida
            reboundStartPosition = rb.position;
            reboundStartRotation = rb.rotation;

            // ✅ Cortamos rotación libre actual
            rb.angularVelocity = Vector3.zero;
        }
    }
}
