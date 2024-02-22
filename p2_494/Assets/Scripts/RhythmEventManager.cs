using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RhythmEventManager : MonoBehaviour
{
    Subscription<MissedEvent> missedEventSub;
    Subscription<HitEvent> hitEventSub;
    Subscription<DeathEvent> deathEventSub;
    List<Sprite> sprites = new List<Sprite>();
    int spawnCount = 0;
    int spriteIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        missedEventSub = EventBus.Subscribe<MissedEvent>(_OnRhythmMissed);
        deathEventSub = EventBus.Subscribe<DeathEvent>(_OnDeath);
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle3"));
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle4"));

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
            SpriteRenderer spriteRenderer = tile.GetComponent< SpriteRenderer>();
            spriteRenderer.sprite = sprites[spriteIdx % sprites.Count];
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