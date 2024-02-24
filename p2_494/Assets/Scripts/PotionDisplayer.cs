using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PotionDisplayer : MonoBehaviour
{
    Subscription<UpdatePotionEvent> updatePotionEventSub;

    // Start is called before the first frame update
    void Start()
    {
        updatePotionEventSub = EventBus.Subscribe<UpdatePotionEvent>(_OnPotionUpdated);

    }

    void _OnPotionUpdated(UpdatePotionEvent e)
    {
        GetComponent<TextMeshProUGUI>().text = "x" + e.numPotions;
    }

}
