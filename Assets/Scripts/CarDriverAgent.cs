using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarDriverAgent : Agent
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;

    private CarControllerWheel car;
    private Rigidbody rb;

    private float stuckTimer = 0f;
    private float prevDistToNext = 0f;
    
    private void Start()
    {
        car = GetComponent<CarControllerWheel>();
        rb  = GetComponent<Rigidbody>();
        
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint   += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
            AddReward(-1f);
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
            AddReward(+3f);
    }

    public void ResetPositions()
    {
        transform.position = spawnPosition.position;
        transform.forward  = spawnPosition.forward;
    }
    
    public override void OnEpisodeBegin()
    {
        stuckTimer = 0f;
        trackCheckpoints.RefreshCheckpoints();
        ResetPositions();
        
        // TRAINING
        /*// 30% of episodes: start in a random orientation
        float angle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * spawnPosition.rotation;*/

        //rb.linearVelocity  = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;

        trackCheckpoints.ResetCheckpoint(transform);
        trackCheckpoints.ResetAllCheckpointsVisual();

        car.StopCompletely();

        // Init distance to next checkpoint for progress reward
        var next = trackCheckpoints.GetNextCheckpoint(transform);
        prevDistToNext = Vector3.Distance(transform.position, next.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // --- NEXT CHECKPOINT ---
        var next = trackCheckpoints.GetNextCheckpoint(transform);
        var toNext = next.position - transform.position;
        var toNextLocal = transform.InverseTransformDirection(toNext.normalized);

        float dotForward = Vector3.Dot(transform.forward, toNext.normalized);
        sensor.AddObservation(dotForward);

        // Direction to next checkpoint (local)
        sensor.AddObservation(toNextLocal.x);
        sensor.AddObservation(toNextLocal.z);

        // Distance to next checkpoint (scaled)
        sensor.AddObservation(Mathf.Clamp01(toNext.magnitude / 50f));

        // --- SECOND NEXT CHECKPOINT (LOOKAHEAD) ---
        var next2 = trackCheckpoints.GetSecondNextCheckpoint(transform);
        var toNext2 = next2.position - transform.position;
        var toNext2Local = transform.InverseTransformDirection(toNext2.normalized);

        sensor.AddObservation(toNext2Local.x);
        sensor.AddObservation(toNext2Local.z);

        // --- VELOCITY (local) ---
        var velLocal = transform.InverseTransformDirection(rb.linearVelocity);
        sensor.AddObservation(velLocal.x); // sideways
        sensor.AddObservation(velLocal.z); // forward
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!RaceManager.instance.RaceStarted || RaceManager.instance.IsPaused)
            return;

        float forwardAmount = 0f;
        float turnAmount    = 0f;

        // throttle / brake
        switch (actions.DiscreteActions[0])
        {
            case 1: forwardAmount =  1f; break; // accelerate
            case 2: forwardAmount = -1f; break; // brake / reverse
        }

        // steering
        switch (actions.DiscreteActions[1])
        {
            case 1: turnAmount =  1f; break; // right
            case 2: turnAmount = -1f; break; // left
        }

        bool tryingToMove = Mathf.Abs(forwardAmount) > 0.01f || Mathf.Abs(turnAmount) > 0.01f;

        // --- NAVIGATION REWARDS ---

        var next = trackCheckpoints.GetNextCheckpoint(transform);
        Vector3 toNext = next.position - transform.position;
        float distToNext = toNext.magnitude;
        Vector3 dirToNext = toNext.normalized;

        // 1) Alignment
        float alignDot = Vector3.Dot(transform.forward, dirToNext);   // -1..1
        AddReward(alignDot * 0.002f);

        // 2) Progress towards checkpoint
        float progress = prevDistToNext - distToNext; // >0 if closer
        AddReward(progress * 0.02f);
        prevDistToNext = distToNext;

        // Small time penalty
        AddReward(-0.001f);

        // --- STUCK DETECTION USING car.IsStopped() ---

        if (tryingToMove && car.IsStopped(0.5f))
        {
            stuckTimer += Time.fixedDeltaTime;

            // penalty while stuck (after a short grace time)
            if (stuckTimer > 1.0f)
                AddReward(-0.02f);
        }
        else
        {
            // reward escaping a stuck state
            if (stuckTimer > 0.2f && tryingToMove)
                AddReward(+0.3f);

            stuckTimer = 0f;
        }

        // Apply actions to car: (steer, throttle, brake)
        car.SetInputs(turnAmount, forwardAmount, 0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (!RaceManager.instance.RaceStarted || RaceManager.instance.IsPaused)
            return;

        int forwardAction = 0;
        if (Input.GetKey(KeyCode.UpArrow))   forwardAction = 1;
        if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 2;

        int turnAction = 0;
        if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow))  turnAction = 2;

        var discrete = actionsOut.DiscreteActions;
        discrete[0] = forwardAction;
        discrete[1] = turnAction;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Wall wall))
            AddReward(-0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Wall wall))
            AddReward(-0.01f);
    }
}
