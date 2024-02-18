using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RhythmEventManager : MonoBehaviour
{
    Subscription<MissedEvent> missedEventSub;
    Subscription<HitEvent> hitEventSub;
    GameObject missVisual;
    GameObject hitVisual;

    // Start is called before the first frame update
    void Start()
    {
        missedEventSub = EventBus.Subscribe<MissedEvent>(_OnRhythmMissed);
        hitEventSub = EventBus.Subscribe<HitEvent>(_OnRhythmHit);
        missVisual = ResourceLoader.GetPrefab("missVisual");
        hitVisual = ResourceLoader.GetPrefab("hitVisual");
    }

    void _OnRhythmMissed(MissedEvent e)
    {
        // Find the closest rhythm gameobject to the player + destroy it
        GameObject player = GameObject.Find("Player");
        GameObject[] rhythmObjects = GameObject.FindGameObjectsWithTag("rhythm");
        if (rhythmObjects.Length > 0)
        {
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
            Vector3 visualSpawnPos = toDestroy.transform.position;
            visualSpawnPos.y += 0.5f;
            Destroy(toDestroy);
            GameObject.Instantiate(missVisual, visualSpawnPos, Quaternion.identity);
        }
    }

    void _OnRhythmHit(HitEvent e)
    {
        GameObject toDestroy = e.tileHit;
        Vector3 visualSpawnPos = toDestroy.transform.position;
        visualSpawnPos.y += 0.5f;
        Destroy(toDestroy);
        GameObject.Instantiate(hitVisual, visualSpawnPos, Quaternion.identity);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(missedEventSub);
        EventBus.Unsubscribe(hitEventSub);
    }
}

public class MissedEvent
{

}

public class HitEvent
{
    public GameObject tileHit;
    public HitEvent(GameObject _tileHit) { tileHit = _tileHit; }
}