using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OnTrigger : MonoBehaviour
{
    Sprite obstacleSprite;
    // Start is called before the first frame update
    void Start()
    {
        obstacleSprite = ResourceLoader.GetSprite("obstacle4");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player entered a rhythm tile
        if (other.gameObject.CompareTag("tile"))
        {
            Sprite currSprite = other.gameObject.GetComponent<SpriteRenderer>().sprite;
            if(currSprite == obstacleSprite)
            {
                Debug.Log("death event!");
            }
        }
    }

}
