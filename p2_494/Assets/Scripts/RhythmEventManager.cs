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

    // For Obstacle Switching
    List<Sprite> switchSprites = new List<Sprite>();
    int spriteIdx = 0;
    bool movementEnabled = false;

    // Grid Sprites
    Sprite missGridSprite;
    Sprite hitGridSprite;
    Sprite defaultGridSprite;
    // For Determining where to spawn music note in grid system
    public LayerMask mask;
    public float noteDurationBeforeMiss = 1.3f;
    int numMisses = 0;
    public int freeMisses;

    AudioClip damageSfx;
    AudioClip hitSfx;

    Subscription<DisplayHitOrMissEvent> displayHitOrMissSub;

    void Start()
    {
        switchSprites.Add(ResourceLoader.GetSprite("obstacle4"));
        switchSprites.Add(ResourceLoader.GetSprite("tile1"));
        secPerBeat = 60f / bpm;

        missGridSprite = ResourceLoader.GetSprite("missSprite");
        hitGridSprite = ResourceLoader.GetSprite("hitSprite");
        defaultGridSprite = ResourceLoader.GetSprite("defaultSprite");

        hitSfx = ResourceLoader.GetAudioClip("blipSfx");
        damageSfx = ResourceLoader.GetAudioClip("damageSfx");

        displayHitOrMissSub = EventBus.Subscribe<DisplayHitOrMissEvent>(_OnDisplayHitOrMiss);

        if(wasSceneReloaded)
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

    // Grid Music Note Functions
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
                if(tile.GetComponent<SpriteRenderer>().sprite != defaultGridSprite)
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
            rhythm.GetComponent<MusicNote>().StartDestroyRoutine(secPerBeat * noteDurationBeforeMiss);
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
            numMisses++;
            AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
            tile.GetComponent<SpriteRenderer>().sprite = missGridSprite;
            if(wasSceneReloaded || numMisses > freeMisses)
            {
                // Reduce health on every other miss
                if (numMisses % 2 == 0)
                {
                    EventBus.Publish<ReduceHealth>(new ReduceHealth());
                }
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(hitSfx, Camera.main.transform.position);
            tile.GetComponent<SpriteRenderer>().sprite = hitGridSprite;
        }

        yield return new WaitForSeconds(0.3f);

        tile.GetComponent<SpriteRenderer>().sprite = defaultGridSprite;
    }

    public void Restart()
    {
        StartCoroutine(DeleteOneShotAudio());
    }
    IEnumerator DeleteOneShotAudio()
    {
        // Ocassionally, One shot audio objects were not being cleaned up correctly
        while(GameObject.Find("One shot audio") != null)
        {
            Destroy(GameObject.Find("One shot audio"));
            yield return null;
        }

        wasSceneReloaded = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(displayHitOrMissSub);
    }
}