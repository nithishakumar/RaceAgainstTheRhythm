using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    Sprite obstacleSprite;
    Dictionary<GameObject, Boolean> tileStates = new Dictionary<GameObject, Boolean>();
    public float bpm;
    float secPerBeat;

    void Start()
    {
        obstacleSprite = ResourceLoader.GetSprite("obstacle4");
        secPerBeat = 60f / bpm;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player entered a tile
        if (other.gameObject.CompareTag("tile") || other.gameObject.CompareTag("safeTile"))
        {
            tileStates[other.gameObject] = true;
            // Start waiting for the player to touch a spike sprite
            StartCoroutine(DetectSprite(other.gameObject));
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player left a tile
        if (other.gameObject.CompareTag("tile") || other.gameObject.CompareTag("safeTile"))
        {
            // Stop waiting for the player to touch a spike sprite
            tileStates[other.gameObject] = false;

        }
    }

    IEnumerator DetectSprite(GameObject tile)
    {
        while (tileStates[tile])
        {
            Sprite currSprite = tile.GetComponent<SpriteRenderer>().sprite;
            if (currSprite == obstacleSprite)
            {
                EventBus.Publish<ReduceHealth>(new ReduceHealth());
                // Don't reduce health multiple times in a row
                yield return new WaitForSeconds(secPerBeat);
            }
            yield return null;
        }
    }
}

public class ReduceHealth
{
}

