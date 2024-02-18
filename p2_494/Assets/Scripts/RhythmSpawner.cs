using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class CallToLocations
{
    public Transform[] spawnLocations;
}

public class RhythmSpawner : MonoBehaviour
{
    public CallToLocations[] locations;
    Subscription<RhythmSpawnEvent> rhythmEventSub;

    List<GameObject> notes = new List<GameObject>();

    void Start()
    {
        rhythmEventSub = EventBus.Subscribe<RhythmSpawnEvent>(SpawnTile);
        notes.Add(ResourceLoader.GetPrefab("musicNote1"));
        StartCoroutine(SpawnTiles());
    }

    void SpawnTile(RhythmSpawnEvent e)
    {
        if (e.numSpawnCall < 2)
        {
            
            Transform[] spawnLocations = locations[e.numSpawnCall].spawnLocations;
            for(int i = 0; i < spawnLocations.Length; i++)
            {
                Debug.Log("entered");
                GameObject.Instantiate(notes[0], spawnLocations[i].position, Quaternion.identity);
            }

        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(rhythmEventSub);
    }

    IEnumerator SpawnTiles()
    {
        int spawnCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(2.5f * 0.6f);
            EventBus.Publish<RhythmSpawnEvent>(new RhythmSpawnEvent(spawnCount));
            spawnCount++;
            yield return null;
        }
    }
}
