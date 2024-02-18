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

    // Stores all the beats that are yet to be missed
    List<int> correctBeats = new List<int>();

    // Counts the number of times the 4th beat has occured
    int fourthBeatCount = 0;

    // Number of beats to ignore before starting the game
    public int numBeatsToIgnore = 4;

    public bool playerOnRhythmTile = false;

    public GameObject currentTile;

    HashSet<int> ignoreBeats = new HashSet<int>();

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
        float duration = secPerBeat * numBeatsToIgnore;
        
        CharacterMovement movement = GameObject.Find("Player").GetComponent<CharacterMovement>();
        movement.enabled = false;
        // Ignore first X beats and then start the game
        yield return new WaitForSeconds(duration - 0.5f);
        movement.enabled = true;
        yield return new WaitForSeconds(0.5f);
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
                if (!ignoreBeats.Contains(fourthBeatCount))
                { 
                    // current beat is at correct state
                    correctBeats.Add(fourthBeatCount);
                    StartCoroutine(MissRoutine(fourthBeatCount));
                }
            }
            
            yield return null;
        }
    }


    IEnumerator MissRoutine(int beat)
    {
        yield return new WaitForSeconds(0.7f);
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
                        // If at least one of the beats are in correct state and player is on a rhythm object, accept correct input
                        if (correctBeats.Count > 0 && playerOnRhythmTile)
                        {
                            Debug.Log("perfect!");
                            // finish the first beat's state
                            correctBeats.RemoveAt(0);
                            EventBus.Publish<HitEvent>(new HitEvent(currentTile));
                            playerOnRhythmTile = false;
                            break;
                        }
                        else if (correctBeats.Count <= 0)
                        {
                            Debug.Log("incorrect: Spacebar pressed too early " + fourthBeatCount);
                            ignoreBeats.Add(fourthBeatCount + 1);
                            EventBus.Publish<MissedEvent>(new MissedEvent());
                            break;
                        }
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            if (!loopEntered && Input.GetKeyUp(KeyCode.Space))
            {
                // TODO: End game here
                Debug.Log("spacebar pressed when player wasn't on correct tile");
            }
            loopEntered = false;
            yield return null;
        }
    }
}
