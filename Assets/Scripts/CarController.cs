using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Control Mode")]
    [Tooltip("If true, car is controlled by player input (keyboard). If false, ML-Agent controls it.")]
    public bool usePlayerInput = false;
    public bool isPlayer = false;

    [Header("Car Settings")]
    public float maxForwardSpeed = 5f;
    public float maxReverseSpeed = 3f;
    public float acceleration = 20f;
    public float deceleration = 20f;
    public float maxSteerAngle = 60f;
    public float steerSpeed = 20f;
    public float speedSteerFactor = 0.8f;

    public int carIndex;
    public bool forwardIsZ = true;

    private Rigidbody rb;
    private float currentSpeed;
    private float currentSteer;
    private float forwardInput;
    private float turnInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        // Target speed (forwardInput < 0 means reverse)
        float targetSpeed = (forwardInput > 0f)
            ? forwardInput * maxForwardSpeed
            : (forwardInput < 0f) ? forwardInput * maxReverseSpeed : 0f;

        // Smooth accel/decel
        float accel = (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed) || Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed))
            ? acceleration : deceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);

        // Steering
        currentSteer = Mathf.MoveTowards(currentSteer, turnInput * maxSteerAngle, steerSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 localForward = forwardIsZ ? transform.forward : transform.up;
        Vector3 desiredVelocity = localForward * currentSpeed;

        // Preserve gravity while controlling horizontal velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontal = Vector3.Lerp(new Vector3(velocity.x, 0f, velocity.z), desiredVelocity, 0.25f);
        rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);

        // === Cap Max Speed ===
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = flatVel.magnitude;

        float maxSpeed = (currentSpeed >= 0f) ? maxForwardSpeed : maxReverseSpeed;
        if (speed > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        // Steering (safe for forward & reverse)
        if (Mathf.Abs(currentSteer) > 0.01f && Mathf.Abs(currentSpeed) > 0.05f)
        {
            float steerFactor = (Mathf.Abs(currentSpeed) / maxForwardSpeed) * speedSteerFactor;
            float rotationAmount = currentSteer * steerFactor * Mathf.Sign(currentSpeed) * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
    
    // === ML-Agent API ===
    public void SetInputs(float forwardAmount, float turnAmount)
    {
        if (usePlayerInput) return;
        forwardInput = Mathf.Clamp(forwardAmount, -1f, 1f);
        turnInput = Mathf.Clamp(turnAmount, -1f, 1f);
    }

    public void StopCompletely()
    {
        currentSpeed = 0f;
        currentSteer = 0f;
        forwardInput = 0f;
        turnInput = 0f;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
