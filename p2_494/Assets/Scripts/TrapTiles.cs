using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTiles : MonoBehaviour
{
    bool moved = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TrapWhenPlayerIsClose());
    }

    IEnumerator TrapWhenPlayerIsClose()
    {
        while (!moved)
        {
            GameObject[] gridTiles = GameObject.FindGameObjectsWithTag("grid");
            foreach (var tile in gridTiles)
            {
                if ((tile.transform.position - transform.position).magnitude <= 0.06)
                {
                    Debug.Log("found");
                    Trap();
                    EventBus.Publish<OnTrappedEvent>(new OnTrappedEvent());
                    break;
                }
            }
            yield return null;
        }
    }

    void Trap()
    {
        GameObject.Find("Player").GetComponent<CharacterMovement>().enabled = false;
        moved = true;
        GameObject[] traps = GameObject.FindGameObjectsWithTag("trap");
        foreach (var trap in traps)
        {
            trap.GetComponent<Collider>().isTrigger = false;
            trap.layer = 3;
            trap.GetComponent<SpriteRenderer>().sprite = ResourceLoader.GetSprite("boxObstacle");
        }
        GameObject.Find("Player").GetComponent<CharacterMovement>().enabled = true;
    }
}
