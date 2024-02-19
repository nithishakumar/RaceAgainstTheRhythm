using System.Collections;
using UnityEngine;

public enum BeatStates
{
    Undetected,
    TileSpawned,
    Detected,
    WaitingForHit,
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

    // Indicates if player is on Rhtyhm tile - controlled by OnTrigger.cs
    public bool playerOnRhythmTile = false;

    // The last tile the player was on - controlled by OnTrigger.cs
    public GameObject currentTile;

    // No. of beats in the song
    public float numBeats;
    public float score;

    public float scoreDecrementPerMiss = 2;

    public float lastEventTimeStamp = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        numBeats = Mathf.RoundToInt(musicSource.clip.length / secPerBeat) - numBeatsToIgnore;

        score = numBeats;

        // Add one because arrays are zero indexed
        beats = new BeatStates[(int)numBeats + 1];

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
            {   
                // Store curr position in beats for next frame
                last4thBeat = songPositionInBeats;
                fourthBeatCount++;
                Debug.Log("beat " + fourthBeatCount + " detected");
                // Only process beat if its in tile spawned state
                if (beats[fourthBeatCount] == BeatStates.TileSpawned)
                {
                    beats[fourthBeatCount] = BeatStates.Detected;
                    StartCoroutine(MissRoutine(fourthBeatCount));
                }
            }
            // Space bar was clicked when beat wasn't detected
            // Check that atleast 0.5s has passed since the last event to avoid simultaneous miss + hit events
            else if(Input.GetKeyDown(KeyCode.Space) && (Time.time - lastEventTimeStamp >= secPerBeat || lastEventTimeStamp == 0))
            {
                Debug.Log("should miss: spacebar clicked out of sync!");
                int idx = GetFirstIdxOfBeat(BeatStates.TileSpawned);
                if (idx != -1)
                {
                    beats[idx] = BeatStates.Missed;
                    score -= scoreDecrementPerMiss;
                    Debug.Log("Missed beat " + idx + "! new score: " + score.ToString());
                    EventBus.Publish<MissedEvent>(new MissedEvent(score, numBeats));
                }

            }
            
            yield return null;
        }
    }

    IEnumerator MissRoutine(int beat)
    {
        beats[beat] = BeatStates.WaitingForHit;
        yield return StartCoroutine(WaitForHit());
        // Only miss beat if it is still waiting for hit
        if (beats[beat] == BeatStates.WaitingForHit)
        {
            beats[beat] = BeatStates.Missed;
            score -= scoreDecrementPerMiss;
            Debug.Log("Missed beat " + beat + "! new score: " + score.ToString());
            EventBus.Publish<MissedEvent>(new MissedEvent(score, numBeats));
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // If at least one of the beats are in detected state and player is on a rhythm object, accept correct input
                    int idx = GetFirstIdxOfBeat(BeatStates.WaitingForHit);
                    if (idx != -1)
                    {
                        Debug.Log("perfect!");
                        // finish the first beat's state
                        beats[idx] = BeatStates.Hit;
                        EventBus.Publish<HitEvent>(new HitEvent(currentTile));
                        break;
                    }
                    // Idx will never be -1 because WaitForHit is started only after detecting a beat.
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
   
    
}
