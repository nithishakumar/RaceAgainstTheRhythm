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
    public float songPositionInBeats;

    // How many seconds have passed since the song started
    public float dspSongTime;

    // An AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    public float lastSongPos = 0;

    public UnityEvent Trigger;

    string spaceState = "incorrect";

    List<int> correctBeats = new List<int>();

    int beatCount = 0;

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

        StartCoroutine("InputDetectionRoutine");
        StartCoroutine("SpaceDetection");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator InputDetectionRoutine()
    {
        while (true)
        {
            // Determine how many seconds since the song started
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);

            // Determine how many beats since the song started
            songPositionInBeats = songPosition / secPerBeat;

            if (songPositionInBeats - lastSongPos >= 3.9)
            {   // Store curr position in beats for next frame
                lastSongPos = songPositionInBeats;
                beatCount++;
                Debug.Log("beat " + beatCount + " detected");
                // current beat is at correct state
                correctBeats.Add(beatCount);
                StartCoroutine(MissRoutine(beatCount));
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
