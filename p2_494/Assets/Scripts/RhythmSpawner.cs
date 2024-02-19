using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

// Stores the locations where rhythm objects will be spawned for every call to SpawnTiles
public class CallToLocations
{
    public List<Transform> spawnLocations;
}

public class RhythmSpawner : MonoBehaviour
{
    public CallToLocations[] locations;
    Subscription<RhythmSpawnEvent> rhythmEventSub;
    public float songBpm;
    public float spawnEveryXthBeat = 2.5f;
    float secPerBeat;
    List<GameObject> musicNotes = new List<GameObject>();
    int beats = 1;
    Conductor conductor;
    void Start()
    {
        rhythmEventSub = EventBus.Subscribe<RhythmSpawnEvent>(SpawnTile);
        musicNotes.Add(ResourceLoader.GetPrefab("musicNote1"));
        conductor = GameObject.Find("Conductor").GetComponent<Conductor>();
        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;
        StartCoroutine(SpawnTiles());
    }

    void SpawnTile(RhythmSpawnEvent e)
    {
        if (e.numSpawnCall < locations.Length)
        {
            
            List<Transform> spawnLocations = locations[e.numSpawnCall].spawnLocations;
            for(int i = 0; i < spawnLocations.Count; i++)
            {
                 GameObject.Instantiate(musicNotes[0], spawnLocations[i].position, Quaternion.identity);
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
            yield return new WaitForSeconds(spawnEveryXthBeat * secPerBeat);
            conductor.beats[beats] = BeatStates.TileSpawned;
            beats++;
            EventBus.Publish<RhythmSpawnEvent>(new RhythmSpawnEvent(spawnCount));
            Debug.Log(spawnCount);
            spawnCount++;
            yield return null;
        }
    }
}

public class RhythmSpawnEvent
{
    public int numSpawnCall = 0;

    public RhythmSpawnEvent(int _numSpawnCall) { numSpawnCall = _numSpawnCall; }

}
