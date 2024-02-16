using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    public float bpm;
    AudioSource audioSource;
    public Beat[] beats;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bpm *= audioSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var beat in beats)
        {
            float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * beat.GetBeatLength(bpm));
            beat.CheckForNewBeat(sampledTime);
        }
    }
}

[System.Serializable]
public class Beat
{
    public float steps;
    public UnityEvent trigger;
    public float lastBeat;

    public float GetBeatLength(float bpm)
    {
        // bpm is how many beats are in a minute.
        // We want to know how many beats are in a second. 1 minute has 60 seconds.
        // 60 seconds : bpm beats :: 1 second : ? beats
        // bpm = 60 * ?
        // ? = 60 / bpm
        return 60f / (bpm * (1 / steps));
    }

    public void CheckForNewBeat(float beat)
    {
        // New Beat has Started
        if(Mathf.FloorToInt(beat) != lastBeat)
        {
            lastBeat = Mathf.FloorToInt(beat);
            trigger.Invoke();
        }

    }
}
