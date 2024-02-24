using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOnTrigger : MonoBehaviour
{
    Sprite obstacleSprite;
    public Dictionary<GameObject, Boolean> tileStates = new Dictionary<GameObject, Boolean>();
    public float bpm;
    float secPerBeat;

    void Start()
    {
        obstacleSprite = ResourceLoader.GetSprite("obstacle4");
        secPerBeat = 60f / bpm;
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
        }
        else if (other.gameObject.CompareTag("potion"))
        {
            // do potion activity
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
}

public class ReduceHealth
{
}

