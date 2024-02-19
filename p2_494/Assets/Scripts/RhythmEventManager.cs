using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class RhythmEventManager : MonoBehaviour
{
    Subscription<MissedEvent> missedEventSub;
    Subscription<HitEvent> hitEventSub;
    Subscription<DeathEvent> deathEventSub;
    GameObject missVisual;
    GameObject hitVisual;
    Conductor conductor;

    // Start is called before the first frame update
    void Start()
    {
        missedEventSub = EventBus.Subscribe<MissedEvent>(_OnRhythmMissed);
        hitEventSub = EventBus.Subscribe<HitEvent>(_OnRhythmHit);
        deathEventSub = EventBus.Subscribe<DeathEvent>(_OnDeath);
        missVisual = ResourceLoader.GetPrefab("missVisual");
        hitVisual = ResourceLoader.GetPrefab("hitVisual");
        conductor = GameObject.Find("Conductor").GetComponent<Conductor>(); 
    }

    void _OnRhythmMissed(MissedEvent e)
    {
        // Record event timestamp to prevent simultaneous events
        conductor.lastEventTimeStamp = Time.time;

        GameObject player = GameObject.Find("Player");
        GameObject[] rhythmObjects = GameObject.FindGameObjectsWithTag("rhythm");
        if (rhythmObjects.Length > 0)
        {
            // Find the closest rhythm gameobject to the player + destroy it
            float minDistance = Vector3.Distance(rhythmObjects[0].transform.position, player.transform.position);
            GameObject toDestroy = rhythmObjects[0];
            for (int i = 1; i < rhythmObjects.Length; i++)
            {
                float currDistance = Vector3.Distance(rhythmObjects[i].transform.position, player.transform.position);
                if (currDistance < minDistance)
                {
                    toDestroy = rhythmObjects[i];
                }
            }

            // Spawn miss visual
            Vector3 visualSpawnPos = toDestroy.transform.position;
            visualSpawnPos.y += 0.5f;
            GameObject.Instantiate(missVisual, visualSpawnPos, Quaternion.identity);

            // Update Score
            EventBus.Publish<UpdateScoreEvent>(new UpdateScoreEvent(Mathf.FloorToInt((e.score / e.numBeats) * 100)));

            // Reset variables
            conductor.playerOnRhythmTile = false;
            conductor.currentTile = null;

            // Destroy Tile
            Destroy(toDestroy);
        }
       
    }

    void _OnRhythmHit(HitEvent e)
    {
        // Record event timestamp to prevent simultaneous events
        conductor.lastEventTimeStamp = Time.time;

        GameObject toDestroy = e.tileHit;
        if (toDestroy != null)
        {
            // Spawn hit visual
            Vector3 visualSpawnPos = toDestroy.transform.position;
            visualSpawnPos.y += 0.5f;
            GameObject.Instantiate(hitVisual, visualSpawnPos, Quaternion.identity);

            // Destroy tile
            Destroy(toDestroy);

            // Reset variables
            conductor.playerOnRhythmTile = false;
            conductor.currentTile = null;
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