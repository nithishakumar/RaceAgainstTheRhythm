using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public float bpm;
    private AudioSource audioSource;
    public Intervals[] intervals;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        foreach(var interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
        
    }
}

[System.Serializable]
public class Intervals
{
    public float steps;
    private int lastInterval;
    public UnityEvent trigger;
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
