using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    Conductor conductor;
    // Start is called before the first frame update
    void Start()
    {
        conductor = GameObject.Find("Conductor").GetComponent<Conductor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player entered a rhythm tile
        if (other.gameObject.CompareTag("rhythm"))
        {
            conductor.playerOnRhythmTile = true;
            conductor.currentTile = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("rhythm"))
        {
            Debug.Log("i left the tile");
            conductor.playerOnRhythmTile = false;
            conductor.currentTile = null;
        }
    }

    private void OnDestroy()
    {
        conductor.playerOnRhythmTile = false;
    }

}
