using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleOnTrigger : MonoBehaviour
{
    Sprite obstacleSprite;
    public Dictionary<GameObject, Boolean> tileStates = new Dictionary<GameObject, Boolean>();
    public static Dictionary<string, string> forwardTransition = new Dictionary<string, string>();
    public static Dictionary<string, string> backwardTransition = new Dictionary<string, string>();
    public float bpm;
    float secPerBeat;

    void Start()
    {
        obstacleSprite = ResourceLoader.GetSprite("obstacle4");
        secPerBeat = 60f / bpm;
        forwardTransition["Room 1"] = "Room 2";
        backwardTransition["Room 2"] = "Room 1";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player entered a switching obstacle tile
        if (other.gameObject.CompareTag("switch"))
        {
            tileStates[other.gameObject] = true;
            // Start waiting for the player to touch a spike sprite
            StartCoroutine(DetectSprite(other.gameObject));
            
        }
        else if (other.gameObject.CompareTag("stairs"))
        {
            // trigger transition event
            Debug.Log("trigger transition");
            // Set scene reloaded to false!
            ForwardTransition();
        }
        else if (other.gameObject.CompareTag("backwardTransition"))
        {
            // trigger transition event
            Debug.Log("trigger transition");
            // Set scene reloaded to false!
            BackwardTransition();
        }
        else if (other.gameObject.CompareTag("potion"))
        {
            // Collect potion
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player left a switching obstacle tile
        if (other.gameObject.CompareTag("switch"))
        {
            // Stop waiting for the player to touch a spike sprite
            tileStates[other.gameObject] = false;

        }
    }

    IEnumerator DetectSprite(GameObject tile)
    {
        while (tileStates[tile])
        {
            Debug.Log("detecting");
            Sprite currSprite = tile.GetComponent<SpriteRenderer>().sprite;
            if (currSprite == obstacleSprite)
            {
                Debug.Log("detected");
                EventBus.Publish<ReduceHealth>(new ReduceHealth());
                // Don't reduce health multiple times in a row
                yield return new WaitForSeconds(secPerBeat * 1.5f);
            }
            yield return null;
        }
    }


    void ForwardTransition()
    {
        RhythmEventManager.wasSceneReloaded = false;
        string currScene = SceneManager.GetActiveScene().name;
        if (forwardTransition.ContainsKey(currScene))
        {
            SceneManager.LoadScene(forwardTransition[currScene]);
        }
    }

    void BackwardTransition()
    {
        RhythmEventManager.wasSceneReloaded = false;
        string currScene = SceneManager.GetActiveScene().name;
        if (backwardTransition.ContainsKey(currScene))
        {
            SceneManager.LoadScene(backwardTransition[currScene]);
        }
    }
}

public class ReduceHealth
{
}

