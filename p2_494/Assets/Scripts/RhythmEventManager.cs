using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmEventManager : MonoBehaviour
{
    Subscription<MissedEvent> missedEventSub;
    // Start is called before the first frame update
    void Start()
    {
        missedEventSub = EventBus.Subscribe<MissedEvent>(_OnRhythmMissed);
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
            Destroy(toDestroy);
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(missedEventSub);
    }
}

public class MissedEvent
{

}
