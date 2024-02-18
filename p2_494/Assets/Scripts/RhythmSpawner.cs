using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmSpawner : MonoBehaviour
{
    public Transform[] locations;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRhythmObjects()
    {
        for(int  i = 0; i < locations.Length; i++) {



            yield return new WaitForSeconds(4f);
        }
    }
}
