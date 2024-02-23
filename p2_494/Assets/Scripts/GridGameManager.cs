using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGameManager : MonoBehaviour
{
    public float bpm;
    float secPerBeat;

    // Grid Sprites
    bool shouldSpawn = false;
    Sprite missGridSprite;
    Sprite hitGridSprite;
    Sprite defaultGridSprite;
    Sprite tileSprite;

    // For Determining where to spawn music note in grid system
    public LayerMask mask;
    public float noteDurationBeforeMiss = 1.3f;
    int numMisses = 0;
    int numHits = 0;
    public int freeMisses;

    // No. of hits required to be released from trap
    public int hitThreshold;

    AudioClip damageSfx;
    AudioClip hitSfx;

    Subscription<DisplayHitOrMissEvent> displayHitOrMissSub;
    Subscription<OnTrappedEvent> onTrappedSub;


    // Start is called before the first frame update
    void Start()
    {
        missGridSprite = ResourceLoader.GetSprite("missSprite");
        hitGridSprite = ResourceLoader.GetSprite("hitSprite");
        defaultGridSprite = ResourceLoader.GetSprite("defaultSprite");
        tileSprite = ResourceLoader.GetSprite("tile1");

        hitSfx = ResourceLoader.GetAudioClip("blipSfx");
        damageSfx = ResourceLoader.GetAudioClip("damageSfx");

        displayHitOrMissSub = EventBus.Subscribe<DisplayHitOrMissEvent>(_OnDisplayHitOrMiss);
        onTrappedSub = EventBus.Subscribe<OnTrappedEvent>(_OnTrapped);

        secPerBeat = 60f / bpm;

    }

    // Grid Music Note Game Functions
    void _OnTrapped(OnTrappedEvent e)
    {
        StartCoroutine(StartSpawnAfterDelay());
    }

    IEnumerator StartSpawnAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        shouldSpawn = true;
    }
    public void SpawnMusicNote()
    {
        if (!shouldSpawn)
        {
            return;
        }

        GameObject[] gridTiles = GameObject.FindGameObjectsWithTag("grid");
        List<GameObject> randomCandidates = new List<GameObject>();
        GameObject player = GameObject.Find("Player");
        foreach (var tile in gridTiles)
        {
            Collider[] colliders = Physics.OverlapSphere(tile.transform.position, 0.05f, mask);
            // Ensure there is nothing on the tile and the player isn't too far away from the tile
            if (colliders.Length <= 0 && Vector3.Distance(player.transform.position, tile.transform.position) <= 1.5)
            {
                bool indicator = true;
                if (tile.GetComponent<SpriteRenderer>().sprite != defaultGridSprite)
                {
                    indicator = false;
                }

                GameObject[] musicNotes = GameObject.FindGameObjectsWithTag("rhythm");
                foreach (var note in musicNotes)
                {
                    // Ensure this tile isn't too far away from a previously spawned note that the player has to go to
                    if (Vector3.Distance(note.transform.position, tile.transform.position) >= 1.5)
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
        if (shouldSpawn)
        {
            if (state == "miss")
            {
                numMisses++;
                AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
                tile.GetComponent<SpriteRenderer>().sprite = missGridSprite;
                if (RhythmEventManager.wasSceneReloaded || numMisses > freeMisses)
                {
                    // Reduce health on every other miss
                    if (numMisses % 2 == 0)
                    {
                        EventBus.Publish<ReduceHealth>(new ReduceHealth());
                    }
                }
            }
            // Hit event
            else
            {
                numHits++;
                ReleaseTrap();
                Debug.Log("num hits: " + numHits);
                AudioSource.PlayClipAtPoint(hitSfx, Camera.main.transform.position);
                tile.GetComponent<SpriteRenderer>().sprite = hitGridSprite;
            }

            yield return new WaitForSeconds(0.3f);

            tile.GetComponent<SpriteRenderer>().sprite = defaultGridSprite;
        }
    }

    public void ReleaseTrap()
    {
        if(numHits > hitThreshold)
        {
            Debug.Log("entered release trap");
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("release");
            foreach(var obstacle in obstacles)
            {
                obstacle.GetComponent<SpriteRenderer>().sprite = tileSprite;
                obstacle.GetComponent<Collider>().isTrigger = true;
                // Reset to default layer
                obstacle.layer = 0;
            }
            // Stop spawning tiles
            shouldSpawn = false;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(displayHitOrMissSub);
        EventBus.Unsubscribe(onTrappedSub);
    }

}

public class OnTrappedEvent
{

}
