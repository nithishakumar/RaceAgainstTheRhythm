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
    public float last4thBeat = 0f;

    // The time interval between spawns
    bool spawned = false;

    // Stores all the beats that are yet to be missed
    List<int> correctBeats = new List<int>();

    // Counts the number of times the 4th beat has occured
    int fourthBeatCount = 0;

    // Number of beats to ignore before starting the game
    public int numBeatsToIgnore = 4;

    public bool playerOnRhythmTile = false;
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
                EventBus.Publish<MissedEvent>(new MissedEvent());
            }
        }
    }

    IEnumerator SpaceDetection()
    {
        bool loopEntered = false;
        bool spacePressed = false;
        while (true)
        {
            if(playerOnRhythmTile)
            {
                float timer = 0f;
                // Player has 0.6s to press the spacebar after entering the rhythm tile
                float duration = 0.6f;
                while(playerOnRhythmTile && timer < duration)
                {
                    loopEntered = true;
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        spacePressed = true;
                        // If at least one of the beats are in correct state and player is on a rhythm object, accept correct input
                        if (correctBeats.Count > 0 && playerOnRhythmTile)
                        {
                            Debug.Log("perfect!");
                            // finish the first beat's state
                            correctBeats.RemoveAt(0);
                            playerOnRhythmTile = false;
                            break;
                        }
                        // No correct beats or player is not on a rhythm object
                        else
                        {
                            Debug.Log("incorrect!");
                            EventBus.Publish<MissedEvent>(new MissedEvent());
                            break;
                        }
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            // Player didn't press the space bar within the given duration after entering the tile
            if (loopEntered && !spacePressed)
            {
                Debug.Log("didn't press spacebar on time after entering tile");
                EventBus.Publish<MissedEvent>(new MissedEvent());
                // To ensure the missed event is not triggered multiple times
                playerOnRhythmTile = false;
            }
            else if (!loopEntered && Input.GetKeyUp(KeyCode.Space))
            {
                Debug.Log("spacebar pressed when player wasn't on tile");
            }
            loopEntered = false;
            spacePressed = false;
            yield return null;
        }
    }

   
}
