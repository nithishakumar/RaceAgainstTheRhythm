using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventDestruction : MonoBehaviour
{
    private void Awake()
    {
        if (!RhythmEventManager.wasSceneReloaded)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
