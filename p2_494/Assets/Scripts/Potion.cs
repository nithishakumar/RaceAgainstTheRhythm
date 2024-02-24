using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public int numPotions = 100;
    public bool shouldUse = false;
    public bool usingPotion = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldUse && Input.GetKeyDown(KeyCode.Space) && !usingPotion)
        {
            usingPotion = true;
            StartCoroutine(PotionRoutine());
        }
    }

    IEnumerator PotionRoutine()
    {
        numPotions--;
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
