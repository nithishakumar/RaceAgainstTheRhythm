using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInst : MonoBehaviour
{
    private void Awake()
    {
        if(GameObject.FindGameObjectsWithTag("canvas").Length > 1)
        {
            Destroy(gameObject);

        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
