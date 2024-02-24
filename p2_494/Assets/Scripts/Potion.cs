using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public int numPotions = 50;
    public bool shouldUse = false;
    public bool usingPotion = false;
    public bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTime)
        {
            EventBus.Publish<UpdatePotionEvent>(new UpdatePotionEvent(numPotions));
            firstTime = true;
        }

        if (shouldUse && Input.GetKeyDown(KeyCode.Space) && !usingPotion && numPotions > 0)
        {
            usingPotion = true;
            StartCoroutine(PotionRoutine());
        }
    }

    IEnumerator PotionRoutine()
    {
        numPotions--;
        EventBus.Publish<UpdatePotionEvent>(new UpdatePotionEvent(numPotions));
        GetComponent<Animate>().StopAnimation();

        // Flash Sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;

        GetComponent<Animate>().RestartAnimation();
        usingPotion = false;
    }
}

public class UpdatePotionEvent
{
    public int numPotions;

    public UpdatePotionEvent(int _numPotions)
    {
        numPotions = _numPotions;
    }
}
