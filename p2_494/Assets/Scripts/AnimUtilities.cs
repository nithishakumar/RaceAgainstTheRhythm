using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimUtilities
{
    public static IEnumerator Animate(System.Func<bool> condition, GameObject toAnimate, Sprite[] sprites, float animationDelay = 0.25f)
    {
        SpriteRenderer spriteRenderer = toAnimate.GetComponent<SpriteRenderer>();
        Sprite currSprite = spriteRenderer.sprite;

        // If we're in the first frame already, we need to start from displaying the second frame
        int currentIndex = 0;
        for(int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == currSprite)
            {
                currentIndex = i;
            }
        }
        while (condition())
        {
            currentIndex = (currentIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentIndex];
            yield return new WaitForSeconds(animationDelay);
        }
    }
}
