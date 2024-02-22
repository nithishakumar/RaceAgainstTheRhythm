using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RhythmEventManager : MonoBehaviour
{
    Subscription<MissedEvent> missedEventSub;
    Subscription<HitEvent> hitEventSub;
    Subscription<DeathEvent> deathEventSub;

    public List<Transform> spawnLocations;
    List<Sprite> sprites = new List<Sprite>();

    int spawnLocationIdx = 0;
    int spriteIdx = 0;
    int callCount = 0;

    Sprite safeTileSprite;
    Sprite glowSafeTileSprite;

    // Start is called before the first frame update
    void Start()
    {
        missedEventSub = EventBus.Subscribe<MissedEvent>(_OnRhythmMissed);
        deathEventSub = EventBus.Subscribe<DeathEvent>(_OnDeath);
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle3"));
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle4"));
        safeTileSprite = ResourceLoader.GetSprite("obstacle1");
        glowSafeTileSprite = ResourceLoader.GetSprite("obstacle0");
    }

    void _OnRhythmMissed(MissedEvent e)
    {
       
    }

    public void ChangeTileSprite()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
        spriteIdx++;
        foreach (var tile in tiles)
        {
            if (tile.CompareTag("tile"))
            {
                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[spriteIdx % sprites.Count];
            }
            
        }
    }

    public void DespawnOldTile()
    {
        GameObject[] safeTiles = GameObject.FindGameObjectsWithTag("safeTile");
        foreach(var safeTile in safeTiles)
        {
            safeTile.tag = "tile";
            SpriteRenderer spriteRenderer = safeTile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = safeTileSprite;
        }
    }

    public void SpawnTile()
    {
        DespawnOldTile();

        if(spawnLocationIdx < spawnLocations.Count)
        {
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
            foreach(var tile in tiles)
            {
                if(tile.transform.position == spawnLocations[spawnLocationIdx].position)
                {
                    tile.tag = "safeTile";
                    SpriteRenderer spriteRenderer =  tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = glowSafeTileSprite;
                }
            }

            spawnLocationIdx++;
        }
    }

    void _OnDeath(DeathEvent e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(missedEventSub);
        EventBus.Unsubscribe(hitEventSub);
        EventBus.Unsubscribe(deathEventSub);
    }
}

public class MissedEvent
{
    public float score;
    public float numBeats;
    public MissedEvent(float _score, float _numBeats) { score = _score; numBeats = _numBeats; }
}

public class HitEvent
{
    public GameObject tileHit;
    public HitEvent(GameObject _tileHit) { tileHit = _tileHit; }
}

public class DeathEvent
{

}