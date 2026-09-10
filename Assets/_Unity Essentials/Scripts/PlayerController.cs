using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves forward/backward and rotates with WASD/Arrow keys.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("Forward/back speed (units/sec).")]
    public float speed = 5.0f;

    [Tooltip("Turn speed (degrees/sec).")]
    public float rotationSpeed = 120.0f;

    [Tooltip("How quickly the player reaches the requested speed.")]
    public float acceleration = 20.0f;

    [Tooltip("How quickly the player stops when the key is released.")]
    public float deceleration = 30.0f;

    [Tooltip("Lower values make input and turning feel more responsive.")]
    public float smoothingTime = 0.08f;

    public float jumpForce = 5.0f;

    private Rigidbody rb;
    private Vector2 rawInput;
    private Vector2 smoothedInput;
    private Vector2 inputSmoothingVelocity;
    private float forwardSpeed;
    private float turnSpeed;
    private float turnSmoothingVelocity;
    private bool jumpQueued;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogWarning("PlayerController needs a Rigidbody.");

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            rawInput = Vector2.zero;
            return;
        }

        rawInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            rawInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            rawInput.y = -1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            rawInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            rawInput.x = 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpQueued = true;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        float fixedDeltaTime = Time.fixedDeltaTime;
        smoothedInput = Vector2.SmoothDamp(
            smoothedInput,
            rawInput,
            ref inputSmoothingVelocity,
            smoothingTime,
            Mathf.Infinity,
            fixedDeltaTime);

        float targetForwardSpeed = smoothedInput.y * speed;
        float speedChange = Mathf.Abs(targetForwardSpeed) > Mathf.Abs(forwardSpeed)
            ? acceleration
            : deceleration;
        forwardSpeed = Mathf.MoveTowards(
            forwardSpeed,
            targetForwardSpeed,
            speedChange * fixedDeltaTime);

        Vector3 movement = transform.forward * forwardSpeed * fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float turnDirection = smoothedInput.x;
        if (forwardSpeed < 0f)
            turnDirection = -turnDirection;

        float targetTurnSpeed = turnDirection * rotationSpeed;
        turnSpeed = Mathf.SmoothDamp(
            turnSpeed,
            targetTurnSpeed,
            ref turnSmoothingVelocity,
            smoothingTime,
            Mathf.Infinity,
            fixedDeltaTime);

        Quaternion turnRotation = Quaternion.Euler(0f, turnSpeed * fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        if (jumpQueued)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpQueued = false;
        }
    }
}
