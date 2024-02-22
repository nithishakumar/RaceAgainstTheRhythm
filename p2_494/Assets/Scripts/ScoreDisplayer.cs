using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    Subscription<ReduceHealth> reduceHealthSub;
    List<Sprite> sprites = new List<Sprite>();
    AudioClip damageSfx;
    Image image;
    int healthIdx = 0;

    void Start()
    {
        reduceHealthSub = EventBus.Subscribe<ReduceHealth>(_OnReduceHealth);
        damageSfx = ResourceLoader.GetAudioClip("damageSfx");
        for(int i = 1; i <= 8; i++)
        {
            sprites.Add(ResourceLoader.GetSprite("bar" + i.ToString()));
        }
        image = GetComponent<Image>();
    }

    void _OnReduceHealth(ReduceHealth e)
    {
        if(healthIdx <= sprites.Count - 2)
        {
            image.sprite = sprites[healthIdx];
            AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
            StartCoroutine(OnDamage());
            healthIdx++;
        }
        else if (healthIdx == sprites.Count - 1)
        {
            if (image.sprite != sprites[healthIdx])
            {
                AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
                image.sprite = sprites[healthIdx];
                OnDeath();
            }
        }
    }

    void OnDeath()
    {
        GameObject beatManager = GameObject.Find("BeatManager");
        if(beatManager != null)
        {
            // Stop audio source
            Destroy(beatManager);
            GameObject player = GameObject.Find("Player");
            // Stop player movement animaton
            player.GetComponent<Animate>().StopAnimation();
            // Disable player movement
            player.GetComponent<CharacterMovement>().enabled = false;
        }
    }

    IEnumerator OnDamage()
    {
        GameObject player = GameObject.Find("Player");
        // Stop player movement animaton
        player.GetComponent<Animate>().StopAnimation();
        // Flash Sprite
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Animate>().RestartAnimation();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(reduceHealthSub);
    }
}