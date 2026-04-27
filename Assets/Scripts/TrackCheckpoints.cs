using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackCheckpoints : MonoBehaviour
{
    [SerializeField] private List<Transform> carTransformList;
    [SerializeField] private Transform checkpointsTransform;

    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointIndexList;
    private bool hasPlayerWon = false;
    
    private void Awake()
    {
        RefreshCheckpoints();
    }

    public void RefreshCheckpoints()
    {
        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpoint in checkpointsTransform)
        {
            var checkpointSingle = checkpoint.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointIndexList = new List<int>();
        foreach (var car in carTransformList)
        {
            nextCheckpointIndexList.Add(0);
        }
    }
    
    public event EventHandler<CarCheckPointEventArgs> OnCarCorrectCheckpoint;
    public event EventHandler<CarCheckPointEventArgs> OnCarWrongCheckpoint;

    public void CarThroughCheckpoint(CheckpointSingle checkpoint, Transform carTransform)
    {
        var carIndex = carTransformList.IndexOf(carTransform);
        var nextCheckPointIndex = nextCheckpointIndexList[carIndex];
        
        if (checkpointSingleList.IndexOf(checkpoint) == nextCheckPointIndex)
        {
            var correctCheckpoint = checkpointSingleList[nextCheckPointIndex];
            correctCheckpoint.Hide();

            nextCheckpointIndexList[carIndex] = (nextCheckPointIndex + 1) % checkpointSingleList.Count;

            OnCarCorrectCheckpoint?.Invoke(this, new CarCheckPointEventArgs
            {
                carTransform = carTransform
            });
            
            if (nextCheckpointIndexList[carIndex] == 0)
            {
                // Car has reached the final checkpoint and completed a lap
                var agent = carTransform.GetComponent<CarDriverAgent>();
                if (agent != null)
                {
                    hasPlayerWon = agent.GetComponent<CarControllerWheel>().IsPlayer;
                    agent.AddReward(5f);
                }
                
                OnGameEndEvent();
            }
        }
        else
        {
            OnCarWrongCheckpoint?.Invoke(this, new CarCheckPointEventArgs
            {
                carTransform = carTransform
            });

            var correctCheckpoint = checkpointSingleList[nextCheckPointIndex];
            //correctCheckpoint.Show();
        }
    }

    public void OnGameEndEvent()
    {
        // TRAINING 
        EndCarEpisodes();
        
        // ACTUAL
        /*RaceManager.instance.TogglePause(false);
        StartCoroutine(MessageShowTime());*/
    }

    public void EndCarEpisodes()
    {
        foreach (var car in carTransformList)
        {
            var currentCar = car.GetComponent<CarDriverAgent>();
            if (currentCar != null)
            {
                currentCar.EndEpisode();
            }
        }
    }
    
    IEnumerator MessageShowTime()
    {
        RaceManager.instance.ShowMesseage(true, hasPlayerWon);
        
        float t = 3f;
        while (t > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            t--;
        }
        
        RaceManager.instance.EnableRestartMenu();
        RaceManager.instance.ShowMesseage(false, hasPlayerWon);
    }
    
    public void ResetCheckpoint(Transform carTransform)
    {
        var carIndex = carTransformList.IndexOf(carTransform);
        nextCheckpointIndexList[carIndex] = 0;
    }

    public void ResetAllCheckpointsVisual()
    {
        foreach (Transform checkpoint in checkpointsTransform)
        {
            var checkpointSingle = checkpoint.GetComponent<CheckpointSingle>();
            checkpointSingle.Show();
        }
    }
    
    public Transform GetNextCheckpoint(Transform carTransform)
    {
        var carIndex = carTransformList.IndexOf(carTransform);
        return checkpointSingleList[nextCheckpointIndexList[carIndex]].transform;
    }
    
    public Transform GetSecondNextCheckpoint(Transform carTransform)
    {
        var carIndex = carTransformList.IndexOf(carTransform);
        return checkpointSingleList[(nextCheckpointIndexList[carIndex] + 1) % checkpointSingleList.Count].transform;
    }
    
    public class CarCheckPointEventArgs : EventArgs
    {
        public Transform carTransform;
    }
}