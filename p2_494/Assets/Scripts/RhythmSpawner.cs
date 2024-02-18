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
    }

    void SpawnTile(RhythmSpawnEvent e)
    {
        Debug.Log("in spawn tile: " + e.numSpawnCall);
        if (e.numSpawnCall < 2)
        {
            
            Transform[] spawnLocations = locations[e.numSpawnCall].spawnLocations;
            Debug.Log("in spawn tile: " + spawnLocations.Length);
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
}
