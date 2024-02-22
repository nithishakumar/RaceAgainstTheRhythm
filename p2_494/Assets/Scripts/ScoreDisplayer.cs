using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    Subscription<ReduceHealth> reduceHealthSub;
    List<Sprite> sprites = new List<Sprite>();
    Image image;
    int healthIdx = 0;

    void Start()
    {
        reduceHealthSub = EventBus.Subscribe<ReduceHealth>(_OnReduceHealth);
        for(int i = 0; i <= 8; i++)
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
            healthIdx++;
        }
        else if (healthIdx == sprites.Count - 1)
        {
            image.sprite = sprites[healthIdx];
            //  Trigger death
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(reduceHealthSub);
    }
}