using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInst : MonoBehaviour
{
    public static CanvasInst instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
        }
    }
}
