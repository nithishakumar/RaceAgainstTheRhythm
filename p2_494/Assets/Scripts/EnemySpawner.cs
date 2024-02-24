using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public bool shouldSpawn = true;
    public UnityEvent[] eventsToAddLater;
    bool eventsAddedAlready = false;

   public void SpawnEnemy(GameObject enemy)
    {
        if (shouldSpawn)
        {
            GameObject.Instantiate(enemy);
        }
    }

    public void AddEnemies()
    {
        if(eventsAddedAlready)
        {
            return;
        }

        eventsAddedAlready = true;

        BeatManager beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        Intervals interval1 = new Intervals(0.1f, eventsToAddLater[0]);
        Intervals interval2= new Intervals(0.1f, eventsToAddLater[1]);

        Intervals interval3 = new Intervals(0.4f, eventsToAddLater[2]);
        Intervals interval4 = new Intervals(0.4f, eventsToAddLater[3]);

        beatManager.intervals.Add(interval1);
        beatManager.intervals.Add(interval2);
        beatManager.intervals.Add(interval3);
        beatManager.intervals.Add(interval4);
    }
}
