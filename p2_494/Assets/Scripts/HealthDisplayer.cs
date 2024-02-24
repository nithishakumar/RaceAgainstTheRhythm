using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour
{
    Subscription<ReduceHealth> reduceHealthSub;
    Subscription<ResetHealthEvent> resetHealthSub;
    List<Sprite> sprites = new List<Sprite>();
    Sprite fullHealth;
    AudioClip damageSfx;
    Image image;
    public static int healthIdx = 0;

    void Start()
    {
        reduceHealthSub = EventBus.Subscribe<ReduceHealth>(_OnReduceHealth);
        resetHealthSub = EventBus.Subscribe<ResetHealthEvent>(_OnResetHealth);
        fullHealth = ResourceLoader.GetSprite("bar0");
        damageSfx = ResourceLoader.GetAudioClip("damageSfx");
        for (int i = 1; i <= 8; i++)
        {
            sprites.Add(ResourceLoader.GetSprite("bar" + i.ToString()));
        }
        image = GetComponent<Image>();
    }

    public void _OnResetHealth(ResetHealthEvent e)
    {
        // Set health to full
        healthIdx = 0;
        image.sprite = fullHealth;
    }

    void _OnReduceHealth(ReduceHealth e)
    {
        Debug.Log("health idx" + healthIdx);
        if(healthIdx <= sprites.Count - 2)
        {
            image.sprite = sprites[healthIdx];
            //AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
            EventBus.Publish<DamageEvent>(new DamageEvent());
            healthIdx++;
        }
        else if (healthIdx == sprites.Count - 1)
        {
            if (image.sprite != sprites[healthIdx])
            {
                AudioSource.PlayClipAtPoint(damageSfx, Camera.main.transform.position);
                image.sprite = sprites[healthIdx];
                EventBus.Publish<DeathEvent>(new DeathEvent());
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(reduceHealthSub);
        EventBus.Unsubscribe(resetHealthSub);
    }
}

public class ResetHealthEvent
{

}