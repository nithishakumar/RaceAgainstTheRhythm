using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmEventManager : MonoBehaviour
{
    public float bpm;
    public float secPerBeat;

    public List<Transform> spawnLocations;
    List<Sprite> sprites = new List<Sprite>();

    int spawnLocationIdx = 0;
    int spriteIdx = 0;

    Sprite safeTileSprite;
    Sprite glowSafeTileSprite;

    void Start()
    {
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle3"));
        sprites.Add(ResourceLoader.GetSprite("obstacle2"));
        sprites.Add(ResourceLoader.GetSprite("obstacle4"));
        safeTileSprite = ResourceLoader.GetSprite("obstacle1");
        glowSafeTileSprite = ResourceLoader.GetSprite("obstacle0");
        secPerBeat = 60f / bpm;

    }

    public void ChangeTileSprite()
    {
        // Change between different obstacle sprites every beat
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
        // Change tile sprite to safe zone sprite and allow for it to display spikes after a delay
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
        // Display spike obstacles after a delay
        yield return new WaitForSeconds(secPerBeat * 2);
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