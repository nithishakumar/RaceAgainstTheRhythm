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

    public List<Transform> spawnLocations;
    List<Sprite> switchSprites = new List<Sprite>();

    int spriteIdx = 0;
    public LayerMask mask;


    void Start()
    {
        switchSprites.Add(ResourceLoader.GetSprite("obstacle4"));
        switchSprites.Add(ResourceLoader.GetSprite("obstacle1"));
        secPerBeat = 60f / bpm;

    }

    public void SwitchTileSprite()
    {
        // Switch sprites to be obstacles
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("switch");
        spriteIdx++;
        foreach (var tile in tiles)
        {
            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = switchSprites[spriteIdx % switchSprites.Count];
        }
    }

    public void SpawnMusicNote()
    {
        GameObject[] gridTiles = GameObject.FindGameObjectsWithTag("grid");
        List<GameObject> randomCandidates = new List<GameObject>();
        foreach(var tile in gridTiles)
        {
            Collider[] colliders = Physics.OverlapSphere(tile.transform.position, 0.05f, mask);
            if(colliders.Length <= 0)
            {
                randomCandidates.Add(tile);
            }
        }

        Debug.Log("random candidates: " + randomCandidates.Count);
        if (randomCandidates.Count > 0)
        {
            var random = new System.Random();
            int idx = random.Next(randomCandidates.Count);
            GameObject.Instantiate(ResourceLoader.GetPrefab("musicNote1"), 
                randomCandidates[idx].transform.position, Quaternion.identity);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}