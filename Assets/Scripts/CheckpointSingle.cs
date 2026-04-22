using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private TrackCheckpoints trackCheckpoints;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarControllerWheel driver))
        {
            trackCheckpoints.CarThroughCheckpoint(this, other.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}