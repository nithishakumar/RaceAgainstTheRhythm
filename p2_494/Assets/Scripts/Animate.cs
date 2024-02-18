using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    public Coroutine animRoutine;
    public Sprite[] sprites;
    public float animationDelay = 0.25f;

    void Start()
    {
        animRoutine = StartCoroutine(AnimUtilities.Animate(() => true, gameObject,
                                            sprites, animationDelay));
    }

    
}
