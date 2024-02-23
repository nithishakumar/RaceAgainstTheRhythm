using System.Collections;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
    string state = "miss";
    public GameObject tile;

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
            EventBus.Publish<DisplayHitOrMissEvent>(new DisplayHitOrMissEvent(tile, state));
        }        
    }
}
public class DisplayHitOrMissEvent
{
    public GameObject tile;
    public string state;
    public DisplayHitOrMissEvent(GameObject _tile, string _state) { tile = _tile; state = _state; }

}
