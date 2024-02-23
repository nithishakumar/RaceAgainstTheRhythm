using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour
{
    public float x;
    public bool startAutomatically = true;

    void Start()
    {
        StartCoroutine(DestroyRoutine());
        
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(x);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
