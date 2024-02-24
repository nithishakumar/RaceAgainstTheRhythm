using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    Rigidbody rb;
    public int track;
    BeatManager beatManager;
    public float moveSpeed = 10f;
    Vector3 target;
    float secPerBeat;
    public float destructionTimeFactor = 2;
    public Vector3 offset = new Vector3(0.4f, 0, 0f);
    public float speedFactor = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject tile = GameObject.FindGameObjectWithTag("enemyTileTrack" + track.ToString());
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        if (tile != null) {
            secPerBeat = 60f / beatManager.bpm;
            target = tile.transform.position - offset;
            float moveSpeed = Vector3.Distance(target, transform.position) / secPerBeat;
            Debug.Log("movespeed: " + moveSpeed);
            rb.velocity = Vector3.left * moveSpeed * speedFactor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            StartCoroutine(Wait(other.gameObject));
        }
    }

    IEnumerator Wait(GameObject player)
    {
        yield return new WaitForSeconds(0.15f);
        Potion potion = player.GetComponent<Potion>();
        if (!potion.usingPotion)
        {
            EventBus.Publish<ReduceHealth>(new ReduceHealth());
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if(!rb.isKinematic && (transform.position - target).magnitude <= 0.5f)
        {
           rb.velocity = Vector3.zero;
           rb.isKinematic = true;
           StartCoroutine(StartDestruction());
        }
    }

    IEnumerator StartDestruction()
    {
        yield return new WaitForSeconds(secPerBeat * destructionTimeFactor);
        Destroy(gameObject);
    }
}
