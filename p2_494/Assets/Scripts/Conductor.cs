using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Conductor : MonoBehaviour
{
    // Song beats per minute
    // This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat
    public float secPerBeat;

    // Current song position, in seconds
    public float songPosition;

    // Current song position, in beats
    public float songPositionInBeats = 0;

    // How many seconds have passed since the song started
    public float dspSongTime;

    // An AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    // The last position of the 4th beat (in beats)
    float last4thBeat = 0f;

    // The time interval between spawns
    bool spawned = false;

    // Stores all the beats that are yet to be missed
    List<int> correctBeats = new List<int>();

    // Counts the number of times the 4th beat has occured
    int fourthBeatCount = 0;

    // Number of beats to ignore before starting the game
    public int numBeatsToIgnore = 4;

    int spawnCalls = 0;
    // Start is called before the first frame update
    void Start()
    {
        // Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        // Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        // Start the music
        musicSource.Play();

        StartCoroutine(IgnoreXBeats());
    }

    IEnumerator IgnoreXBeats()
    {
        yield return new WaitForSeconds(secPerBeat * numBeatsToIgnore);
        EventBus.Publish<RhythmSpawnEvent>(new RhythmSpawnEvent(spawnCalls));
        spawnCalls++;
        spawned = true;
        StartCoroutine("InputDetectionRoutine");
        StartCoroutine("SpaceDetection");
    }

    IEnumerator InputDetectionRoutine()
    {
        while (true)
        {
            // Determine how many seconds since the song started
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - secPerBeat * numBeatsToIgnore);

            // Determine how many beats since the song started
            songPositionInBeats = songPosition / secPerBeat;

            // Spawn tiles every first beat
            if (Mathf.FloorToInt(songPositionInBeats % 4) == 1 && !spawned)
            {
                Debug.Log("Spawn Tile Now!");
                EventBus.Publish<RhythmSpawnEvent>(new RhythmSpawnEvent(spawnCalls));
                spawnCalls++;
                spawned = true;
            }
            // Allow spawning during next first beat
            if(Mathf.FloorToInt(songPositionInBeats % 4) == 1)
            {
                spawned = false;
            }
            // Perform check every fourth beat
            if (songPositionInBeats - last4thBeat >= 4)
            {   // Store curr position in beats for next frame
                last4thBeat = songPositionInBeats;
                fourthBeatCount++;
                Debug.Log("beat " + fourthBeatCount + " detected");
                // current beat is at correct state
                correctBeats.Add(fourthBeatCount);
                StartCoroutine(MissRoutine(fourthBeatCount));
            }
            
            yield return null;
        }
    }


    IEnumerator MissRoutine(int beat)
    {
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < correctBeats.Count; i++)
        {
            if (correctBeats[i] == beat)
            {
                Debug.Log("Missed beat " + beat + "!");
                correctBeats.RemoveAt(i);
            }
        }
    }

    IEnumerator SpaceDetection()
    {
        while(true)
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                // If at least one of the beats are in correct state, accept correct input
                if (correctBeats.Count > 0)
                {
                    Debug.Log("perfect!");
                    // finish the first beat's state
                    correctBeats.RemoveAt(0);
                }
                else
                {
                    Debug.Log("incorrect!");
                }
            }
            yield return null;
        }
    }

   
}
