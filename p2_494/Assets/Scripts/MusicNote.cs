using System.Collections;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
    string state = "miss";

    public GameObject tile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            state = "hit";
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyRoutine(float x)
    {
        yield return new WaitForSeconds(x);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public void StartDestroyRoutine(float x)
    {
        StartCoroutine(DestroyRoutine(x));
    }

    private void OnDestroy()
    {
        if (tile != null)
        {
            GameObject.Find("RhythmEventManager").GetComponent<RhythmEventManager>().StartGridTileRoutine(tile, state);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
