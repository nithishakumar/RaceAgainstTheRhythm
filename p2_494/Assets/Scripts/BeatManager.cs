using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public float bpm;
    private AudioSource audioSource;
    public List<Intervals> intervals;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        try
        {
            foreach (var interval in intervals)
            {
                float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
                interval.CheckForNewInterval(sampledTime);
            }
        }
        catch(InvalidOperationException e)
        {
            Debug.Log("More enemies were added! List was modified so iteration couldn't be performed.");
        }
        
    }
}

[System.Serializable]
public class Intervals
{
    public float steps;
    private int lastInterval;
    public UnityEvent trigger;

    public Intervals(float _steps, UnityEvent _trigger)
    {
        steps = _steps;
        trigger = _trigger;
    }
    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != lastInterval) {
            lastInterval = Mathf.FloorToInt(interval);
            Debug.Log(interval + " beat detected");
            trigger.Invoke();
            
        }
    }
}
