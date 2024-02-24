using System.Collections;
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
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
