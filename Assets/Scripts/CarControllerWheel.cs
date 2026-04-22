using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarControllerWheel : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft;
    [SerializeField] private WheelCollider rearRight;

    [Header("Wheel Visuals (optional)")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;

    [Header("Steering (Arcade but Stable)")]
    [SerializeField] private float maxSteerAngleLowSpeed = 38f;
    [SerializeField] private float maxSteerAngleHighSpeed = 10f; // safer at high speed
    [SerializeField] private float steerSpeed = 9f;

    [Header("Engine / Drivetrain")]
    [SerializeField] private float maxMotorTorque = 650f;
    [SerializeField] private float maxBrakeTorque = 4500f;
    [SerializeField] private float topSpeedKph = 230f;

    [Header("Stability & Grip")]
    [SerializeField] private float downforce = 50f;
    [SerializeField] private float uprightTorqueStrength = 4200f;
    [SerializeField] private float maxUprightAngle = 55f;

    [Header("Tyre Grip (base)")]
    [SerializeField] private float forwardStiffness = 1.6f;
    [SerializeField] private float sidewaysStiffness = 2.8f;  // a bit more grip

    [Header("Rotation Limits")]
    [SerializeField] private float maxAngularVelocity = 7f;

    [Header("Input")]
    [SerializeField] private bool usePlayerInput = true;
    private float steerInput;
    private float throttleInput; // -1..1
    private float brakeInput;    // 0..1

    private Rigidbody rb;
    private float currentSteerAngle;
    public bool IsPlayer => usePlayerInput;

    // ------------- Public API for AI/ML & helpers -------------
    public void SetInputs(float steer, float throttle, float brake)
    {
        steerInput    = Mathf.Clamp(steer, -1f, 1f);
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        brakeInput    = Mathf.Clamp01(brake);
    }

    // Hard stop: instantly stops physics + wheels
    public void StopCompletely()
    {
        // clear inputs
        steerInput    = 0f;
        throttleInput = 0f;
        brakeInput    = 0f;

        // kill motion
        //rb.linearVelocity  = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;

        // make sure wheels are not trying to drive
        float fullBrake = maxBrakeTorque;

        if (frontLeft != null)
        {
            frontLeft.motorTorque = 0f;
            frontLeft.brakeTorque = fullBrake;
        }
        if (frontRight != null)
        {
            frontRight.motorTorque = 0f;
            frontRight.brakeTorque = fullBrake;
        }
        if (rearLeft != null)
        {
            rearLeft.motorTorque = 0f;
            rearLeft.brakeTorque = fullBrake;
        }
        if (rearRight != null)
        {
            rearRight.motorTorque = 0f;
            rearRight.brakeTorque = fullBrake;
        }
    }

    public float GetSpeedKph()
    {
        return rb.linearVelocity.magnitude * 3.6f;
    }

    public float GetForwardSpeedKph()
    {
        float fwd = Vector3.Dot(rb.linearVelocity, transform.forward);
        return fwd * 3.6f;
    }

    public bool IsStopped(float thresholdKph = 0.5f)
    {
        return GetSpeedKph() < thresholdKph;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation      = RigidbodyInterpolation.Interpolate;
        rb.maxAngularVelocity = maxAngularVelocity;

        rb.linearDamping  = 0.08f;
        rb.angularDamping = 0.5f;

        // Auto COM + vertical offset
        Vector3 com = rb.centerOfMass;
        com.x = 0f;
        com.z = 0f;
        com.y -= 0.25f;
        rb.centerOfMass = com;
    }

    void Start()
    {
        SetupWheelFriction(frontLeft);
        SetupWheelFriction(frontRight);
        SetupWheelFriction(rearLeft);
        SetupWheelFriction(rearRight);
    }

    void SetupWheelFriction(WheelCollider wc)
    {
        if (wc == null) return;

        var f = wc.forwardFriction;
        f.stiffness = forwardStiffness;
        wc.forwardFriction = f;

        var s = wc.sidewaysFriction;
        s.stiffness = sidewaysStiffness;
        wc.sidewaysFriction = s;
    }

    void Update()
    {
        if (!usePlayerInput) return;

        float rawSteer    = Input.GetAxis("Horizontal"); // -1..1
        float rawThrottle = Input.GetAxis("Vertical");   // -1..1

        steerInput = Mathf.Clamp(rawSteer, -1f, 1f);

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (rawThrottle > 0.05f)
        {
            // Forward
            throttleInput = Mathf.Clamp(rawThrottle, 0f, 1f);
            brakeInput    = 0f;
        }
        else if (rawThrottle < -0.05f)
        {
            if (forwardSpeed > 1f)
            {
                // Moving forward -> back = brake
                throttleInput = 0f;
                brakeInput    = -rawThrottle;
            }
            else
            {
                // Reverse
                throttleInput = Mathf.Clamp(rawThrottle, -1f, 0f);
                brakeInput    = 0f;
            }
        }
        else
        {
            throttleInput = 0f;
            brakeInput    = 0f;
        }
    }

    void FixedUpdate()
    {
        float speed    = rb.linearVelocity.magnitude;
        float speedKph = speed * 3.6f;

        ApplySteering(speedKph);
        ApplyMotorAndBrakes(speed, speedKph);

        if (speedKph > 2f)
        {
            ApplyDownforce(speed);
            ApplyUprightStabilizer();
        }

        UpdateWheelVisuals();
    }

    // ------------- Steering -------------
    void ApplySteering(float speedKph)
    {
        float t = Mathf.Clamp01(speedKph / topSpeedKph);
        float maxSteer = Mathf.Lerp(maxSteerAngleLowSpeed, maxSteerAngleHighSpeed, t);
        float target = steerInput * maxSteer;

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, target, steerSpeed * Time.fixedDeltaTime);

        if (frontLeft != null)  frontLeft.steerAngle  = currentSteerAngle;
        if (frontRight != null) frontRight.steerAngle = currentSteerAngle;
    }

    // ------------- Motor + Brakes -------------
    void ApplyMotorAndBrakes(float speed, float speedKph)
    {
        float speedLimiter = Mathf.Clamp01(1.05f - (speedKph / topSpeedKph));

        float motorInput   = Mathf.Clamp(throttleInput, -1f, 1f);
        float forwardInput = Mathf.Max(0f, motorInput);
        float reverseInput = -Mathf.Min(0f, motorInput);

        // Soft launch to avoid low-speed jerk
        const float launchSoftSpeedKph = 8f;
        float launchT     = Mathf.Clamp01(speedKph / launchSoftSpeedKph);
        float launchScale = Mathf.Lerp(0.25f, 1f, launchT);

        float forwardTorque = forwardInput * maxMotorTorque * speedLimiter * launchScale;
        float reverseTorque = reverseInput * maxMotorTorque * 0.7f * speedLimiter * launchScale;

        float motor = forwardTorque - reverseTorque;
        float brake = brakeInput * maxBrakeTorque;

        // Simple engine braking when no input
        bool noThrottle = Mathf.Abs(motorInput) < 0.05f;
        bool noBrake    = brakeInput < 0.05f;

        if (noThrottle && noBrake && speed > 0.2f)
        {
            float engineBrakeFactor = Mathf.Clamp01(speedKph / topSpeedKph);
            float engineBrakeTorque = maxBrakeTorque * 0.25f * engineBrakeFactor;

            brake += engineBrakeTorque;
            motor = 0f;
        }

        if (frontLeft != null)  { frontLeft.motorTorque  = motor; frontLeft.brakeTorque  = brake; }
        if (frontRight != null) { frontRight.motorTorque = motor; frontRight.brakeTorque = brake; }
        if (rearLeft != null)   { rearLeft.motorTorque   = motor; rearLeft.brakeTorque   = brake; }
        if (rearRight != null)  { rearRight.motorTorque  = motor; rearRight.brakeTorque  = brake; }
    }

    // ------------- Downforce -------------
    void ApplyDownforce(float speed)
    {
        rb.AddForce(-transform.up * speed * downforce);
    }

    // ------------- Upright -------------
    void ApplyUprightStabilizer()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > maxUprightAngle)
        {
            Vector3 axis = Vector3.Cross(transform.up, Vector3.up).normalized;
            rb.AddTorque(axis * uprightTorqueStrength, ForceMode.Acceleration);
        }
    }

    // ------------- Wheel visuals -------------
    void UpdateWheelVisuals()
    {
        SetWheel(frontLeft,  frontLeftMesh);
        SetWheel(frontRight, frontRightMesh);
        SetWheel(rearLeft,   rearLeftMesh);
        SetWheel(rearRight,  rearRightMesh);
    }

    void SetWheel(WheelCollider col, Transform mesh)
    {
        if (col == null || mesh == null) return;

        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot * Quaternion.Euler(0f, 0f, 90f); // your 90° offset
    }
}
