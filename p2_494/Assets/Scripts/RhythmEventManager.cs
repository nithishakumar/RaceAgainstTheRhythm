using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmEventManager : MonoBehaviour
{
    public List<Transform> spawnLocations;
    List<Sprite> sprites = new List<Sprite>();

    int spawnLocationIdx = 0;
    int spriteIdx = 0;

    Sprite safeTileSprite;
    Sprite glowSafeTileSprite;

    // Start is called before the first frame update
    void Start()
    {
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle3"));
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle4"));
        safeTileSprite = ResourceLoader.GetSprite("obstacle1");
        glowSafeTileSprite = ResourceLoader.GetSprite("obstacle0");
    }

    public void ChangeTileSprite()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
        spriteIdx++;
        foreach (var tile in tiles)
        {
            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[spriteIdx % sprites.Count];
        }
    }

    public void DespawnOldTile()
    {
        GameObject[] safeTiles = GameObject.FindGameObjectsWithTag("safeTile");
        foreach(var safeTile in safeTiles)
        {
            SpriteRenderer spriteRenderer = safeTile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = safeTileSprite;
            StartCoroutine(Despawn(safeTile));
        }
    }

    IEnumerator Despawn(GameObject safeTile)
    {
        yield return new WaitForSeconds(0.6f);
        safeTile.tag = "tile";
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