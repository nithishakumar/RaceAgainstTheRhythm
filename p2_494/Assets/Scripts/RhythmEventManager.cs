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
    public LayerMask mask;

    List<Sprite> switchSprites = new List<Sprite>();
    int spriteIdx = 0;

    Sprite missSprite;
    Sprite hitSprite;
    Sprite defaultSprite;

    Subscription<DisplayHitOrMissEvent> displayHitOrMissSub;

    void Start()
    {
        switchSprites.Add(ResourceLoader.GetSprite("obstacle4"));
        switchSprites.Add(ResourceLoader.GetSprite("obstacle1"));
        secPerBeat = 60f / bpm;

        missSprite = ResourceLoader.GetSprite("missSprite");
        hitSprite = ResourceLoader.GetSprite("hitSprite");
        defaultSprite = ResourceLoader.GetSprite("defaultSprite");

        displayHitOrMissSub = EventBus.Subscribe<DisplayHitOrMissEvent>(_OnDisplayHitOrMiss);
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
        GameObject player = GameObject.Find("Player");
        foreach(var tile in gridTiles)
        {
            Collider[] colliders = Physics.OverlapSphere(tile.transform.position, 0.05f, mask);
            // Ensure there is nothing on the tile and the player isn't too far away from the tile
            if(colliders.Length <= 0 && Vector3.Distance(player.transform.position, tile.transform.position) <= 1.5)
            {
                bool indicator = true;
                if(tile.GetComponent<SpriteRenderer>().sprite != defaultSprite)
                {
                    indicator = false;
                }

                GameObject[] musicNotes = GameObject.FindGameObjectsWithTag("rhythm");
                foreach(var note in musicNotes)
                {
                    // Ensure this tile isn't too far away from a previously spawned note that the player has to go to
                    if(Vector3.Distance(note.transform.position, tile.transform.position) >= 1.5)
                    {
                        indicator = false;
                        break;
                    }
                }
                if (indicator)
                {
                    randomCandidates.Add(tile);
                }
            }
        }
        Debug.Log("random candidates: " + randomCandidates.Count);
        if (randomCandidates.Count > 0)
        {
            var random = new System.Random();
            int idx = random.Next(randomCandidates.Count);
            GameObject rhythm = GameObject.Instantiate(ResourceLoader.GetPrefab("musicNote1"), 
                randomCandidates[idx].transform.position, Quaternion.identity);
            rhythm.GetComponent<MusicNote>().tile = randomCandidates[idx];
            rhythm.GetComponent<MusicNote>().StartDestroyRoutine(secPerBeat * 1.15f);
        }
    }

    public void _OnDisplayHitOrMiss(DisplayHitOrMissEvent e)
    {
        StartCoroutine(GridTileRoutine(e.tile, e.state));
    }

    IEnumerator GridTileRoutine(GameObject tile, string state)
    {
        if(state == "miss")
        {
            tile.GetComponent<SpriteRenderer>().sprite = missSprite;
        }
        else
        {
            tile.GetComponent<SpriteRenderer>().sprite = hitSprite;
        }

        yield return new WaitForSeconds(0.3f);

        tile.GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(displayHitOrMissSub);
    }
}