using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    public Coroutine animRoutine;
    public Sprite[] sprites;
    bool condition = true;
    public float animationDelay = 0.25f;

    void Start()
    {
        animRoutine = StartCoroutine(AnimUtilities.Animate(() => condition, gameObject,
                                            sprites, animationDelay));
    }
    public void StopAnimation()
    {
        condition = false;
    }
    public void RestartAnimation()
    {
        condition = true;
        animRoutine = StartCoroutine(AnimUtilities.Animate(() => condition, gameObject,
                                            sprites, animationDelay));
    }

    
}
