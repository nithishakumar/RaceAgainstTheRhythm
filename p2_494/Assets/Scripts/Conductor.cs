using System.Collections;
using Unity.VisualScripting;
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

    // Stores all the beats states
    public BeatStates[] beats;

    // Counts the number of times the 4th beat has occured
    public int fourthBeatCount = 0;

    // Number of beats to ignore before starting the game
    public int num4thBeatsToIgnore = 4;

    // Indicates if player is on Rhtyhm tile - controlled by OnTrigger.cs
    public bool playerOnRhythmTile = false;

    // The last tile the player was on - controlled by OnTrigger.cs
    public GameObject currentTile;

    // No. of beats in the song
    public float numBeats;

    public float score;

    public float scoreDecrementPerMiss = 2;

    public float lastEventTimeStamp = 0;

    public Intervals[] intervals;

    public float gameDuration = 20f;

    // Start is called before the first frame update
    void Start()
    {
        // Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        numBeats = Mathf.RoundToInt(musicSource.clip.length / secPerBeat) - num4thBeatsToIgnore;

        score = numBeats;

        // Add one because arrays are zero indexed
        beats = new BeatStates[(int)numBeats + 1];
        StartCoroutine(GameCountdown());
    }


    private void Update()
    {
        foreach (var interval in intervals)
        {
            float sampledTime = (musicSource.timeSamples / (musicSource.clip.frequency * interval.GetIntervalLength(songBpm)));
            bool eventTriggered = interval.CheckForNewInterval(sampledTime, gameObject);

            // Space bar was clicked when beat wasn't detected
            // Check that atleast 0.7s has passed since the last event to avoid simultaneous miss + hit events
            if (!eventTriggered && Input.GetKeyDown(KeyCode.Space) && (Time.time - lastEventTimeStamp >= 0.7f || lastEventTimeStamp == 0))
            {
                // If any of the tiles are waiting for a hit, ignore
                int idx = GetFirstIdxOfBeat(BeatStates.WaitingForHit);
                if (idx != -1) return;
                Debug.Log("should miss: spacebar clicked out of sync!");
                idx = GetFirstIdxOfBeat(BeatStates.TileSpawned);
                if (idx != -1)
                {
                    beats[idx] = BeatStates.Missed;
                    score -= scoreDecrementPerMiss;
                    Debug.Log("Missed beat " + idx + "! new score: " + score.ToString());
                    EventBus.Publish<MissedEvent>(new MissedEvent(score, numBeats));
                }
            }
        }
    }

    private void FourthBeatFunction(int beat)
    {
        // Only process beat if its in tile spawned state
        if (beats[beat] == BeatStates.TileSpawned)
        {
            beats[beat] = BeatStates.Detected;
            StartCoroutine(MissRoutine(beat));
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
        Debug.Log("waiting for hit");
        float timer = 0f;
        // Player has 0.6s to press the spacebar after entering the rhythm tile
        float duration = (secPerBeat * 2);
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
                    // Idx will never be -1 because WaitForHit is started only after detecting a beat
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator GameCountdown()
    {
        yield return new WaitForSeconds(gameDuration);
        int accuracy = Mathf.FloorToInt((score / numBeats) * 100);
        if (accuracy >= 50)
        {
            EventBus.Publish<DisplayFinalScoreEvent>(new DisplayFinalScoreEvent(accuracy));
        }
    }


    [System.Serializable]
    public class Intervals
    {
        public float steps;
        private int lastInterval;
        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * steps);
        }

        public bool CheckForNewInterval(float interval, GameObject conductor)
        {
            if (Mathf.FloorToInt(interval) != lastInterval)
            {

                Conductor comp = conductor.GetComponent<Conductor>();  
                comp.fourthBeatCount++;
                lastInterval = Mathf.FloorToInt(interval);
                Debug.Log(interval + " beat detected");
                if (comp.fourthBeatCount > comp.num4thBeatsToIgnore)
                {
                    comp.FourthBeatFunction(comp.fourthBeatCount);
                }
                return true;
            }
            return false;
        }
    }
}
