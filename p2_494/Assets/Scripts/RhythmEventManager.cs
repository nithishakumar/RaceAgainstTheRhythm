using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RhythmEventManager : MonoBehaviour
{
    public float bpm;
    public float secPerBeat;
    public static bool wasSceneReloaded = false;
    public bool switchingObstacles;

    // For Obstacle Switching
    List<Sprite> switchSprites = new List<Sprite>();
    int spriteIdx = 0;
    bool movementEnabled = false;

    void Start()
    {
        switchSprites.Add(ResourceLoader.GetSprite("obstacle4"));
        switchSprites.Add(ResourceLoader.GetSprite("tile1"));
        secPerBeat = 60f / bpm;

        if (wasSceneReloaded || !switchingObstacles)
        {
            GameObject.Find("Player").GetComponent<CharacterMovement>().enabled = true;
        }
    }

    // Obstacle Switching Function
    public void SwitchTileSprite()
    {
        StartCoroutine(EnableMovmentAfterDelay());

        // Switch sprites to be obstacles
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("switch");
        spriteIdx++;
        foreach (var tile in tiles)
        {
            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = switchSprites[spriteIdx % switchSprites.Count];
        }
    }

    IEnumerator EnableMovmentAfterDelay()
    {
        // If the scene is loaded for the first time, don't allow the player to move until they see the switching obstacle
        if (!wasSceneReloaded && !movementEnabled)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("enable movement rn");
            GameObject.Find("Player").GetComponent<CharacterMovement>().enabled = true;
            movementEnabled = true;
        }
    }
}
