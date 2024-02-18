using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public enum BeatStates
{
    Undetected,
    TileSpawned,
    Detected,
    Hit,
    Missed
}

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
    public float last4thBeat = 0f;

    // Stores all the beats states
    public BeatStates[] beats;

    // Counts the number of times the 4th beat has occured
    public int fourthBeatCount = 0;

    // Number of beats to ignore before starting the game
    public int numBeatsToIgnore = 4;

    public bool playerOnRhythmTile = false;

    public GameObject currentTile;

    // Start is called before the first frame update
    void Start()
    {
        // Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        int numBeats = Mathf.RoundToInt(musicSource.clip.length / secPerBeat) - numBeatsToIgnore;

        beats = new BeatStates[numBeats + 1];

        // Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        // Start the music
        musicSource.Play();

        StartCoroutine(IgnoreXBeats());
    }

    IEnumerator IgnoreXBeats()
    {
        float duration = secPerBeat * numBeatsToIgnore;
        
        CharacterMovement movement = GameObject.Find("Player").GetComponent<CharacterMovement>();
        movement.enabled = false;
        // Ignore first X beats and then start the game
        yield return new WaitForSeconds(duration - 0.5f);
        movement.enabled = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("InputDetectionRoutine");
    }

    IEnumerator InputDetectionRoutine()
    {
        float offset = secPerBeat * numBeatsToIgnore;

        while (true)
        {
            // Determine how many seconds since the song started
            songPosition = (float)(AudioSettings.dspTime - dspSongTime - offset);

            // Determine how many beats since the song started
            songPositionInBeats = songPosition / secPerBeat;

            // Perform check every fourth beat
            if (songPositionInBeats - last4thBeat >= 4)
            {   // Store curr position in beats for next frame
                last4thBeat = songPositionInBeats;
                fourthBeatCount++;
                Debug.Log("beat " + fourthBeatCount + " detected");
                // Only process beat if its in tile spawned state
                if (beats[fourthBeatCount] == BeatStates.TileSpawned)
                {
                    // current beat is at detected state
                    beats[fourthBeatCount] = BeatStates.Detected;
                    StartCoroutine(MissRoutine(fourthBeatCount));
                }
            }
            yield return null;
        }
    }


    IEnumerator MissRoutine(int beat)
    {
        yield return StartCoroutine(WaitForHit());
        // Only miss beat if it is in detected state
        if (beats[beat] == BeatStates.Detected)
        {
            beats[beat] = BeatStates.Missed;
            Debug.Log("Missed beat " + beat + "!");
            EventBus.Publish<MissedEvent>(new MissedEvent(beat));
        }
    }

    int GetFirstIdxOfBeat(BeatStates state)
    {
        for(int i = 0; i <  beats.Length; i++)
        {
            if (beats[i] == state) return i;
        }
        return -1;
    }


    IEnumerator WaitForHit()
    {
        float timer = 0f;
        // Player has 0.6s to press the spacebar after entering the rhythm tile
        float duration = secPerBeat;
        while (timer < duration)
        {
            if (playerOnRhythmTile)
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    // If at least one of the beats are in detected state and player is on a rhythm object, accept correct input
                    int idx = GetFirstIdxOfBeat(BeatStates.Detected);
                    if (idx != -1)
                    {
                        Debug.Log("perfect!");
                        // finish the first beat's state
                        beats[idx] = BeatStates.Hit;
                        EventBus.Publish<HitEvent>(new HitEvent(currentTile));
                        break;
                    }
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
