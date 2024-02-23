using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour
{
    public float x;
    public bool startAutomatically = true;

    void Start()
    {
        if (startAutomatically)
        {
            StartCoroutine(DestroyRoutine(x));
        }

    }

    IEnumerator DestroyRoutine(float x)
    {
        yield return new WaitForSeconds(x);
        Destroy(gameObject);
    }

    public void StartDestroyRoutine(float x)
    {
        StartCoroutine (DestroyRoutine(x));
    }
}
