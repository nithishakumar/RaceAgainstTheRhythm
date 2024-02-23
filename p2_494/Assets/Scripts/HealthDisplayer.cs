using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour
{
    Subscription<ReduceHealth> reduceHealthSub;
    List<Sprite> sprites = new List<Sprite>();
    AudioClip damageSfx;
    Image image;
    public static int healthIdx = 0;

    private void Awake()
    {
        // Set health to 0
        if (RhythmEventManager.wasSceneReloaded)
        {
            healthIdx = 0;
        }
    }

    void Start()
    {
        reduceHealthSub = EventBus.Subscribe<ReduceHealth>(_OnReduceHealth);
        damageSfx = ResourceLoader.GetAudioClip("damageSfx");
        for (int i = 1; i <= 8; i++)
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
    }
}