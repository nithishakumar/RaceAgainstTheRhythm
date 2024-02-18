using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour
{
    public float x;

    void Start()
    {
        StartCoroutine(DestroyRoutine());

    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(x);
        Destroy(gameObject);
    }
}
