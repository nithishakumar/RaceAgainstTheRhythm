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
            EventBus.Publish<DisplayHitOrMissEvent>(new DisplayHitOrMissEvent(gameObject, tile, state));
            gameObject.SetActive(false);
        }
    }

    IEnumerator DestroyRoutine(float x)
    {
        yield return new WaitForSeconds(x);
        if (gameObject != null)
        {
            EventBus.Publish<DisplayHitOrMissEvent>(new DisplayHitOrMissEvent(gameObject, tile, state));
            gameObject.SetActive(false);
        }
    }

    public void StartDestroyRoutine(float x)
    {
        StartCoroutine(DestroyRoutine(x));
    }
}
public class DisplayHitOrMissEvent
{
    public GameObject tile;
    public string state;
    public GameObject note;
    public DisplayHitOrMissEvent(GameObject _note, GameObject _tile, string _state) { note = _note;  tile = _tile; state = _state; }

}
