using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool shouldSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void SpawnEnemy(GameObject enemy)
    {
        if (shouldSpawn)
        {
            GameObject.Instantiate(enemy);
        }
    }
}
