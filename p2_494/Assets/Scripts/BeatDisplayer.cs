using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatDisplayer : MonoBehaviour
{
    int startBeat = 1;
    int beatThreshold = 4;
    Subscription<BeatDisplayEvent> beatDisplayEvent;
    TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        beatDisplayEvent = EventBus.Subscribe<BeatDisplayEvent>(_OnBeatDisplay);
    }

    void _OnBeatDisplay(BeatDisplayEvent e)
    {
        textMeshProUGUI.text = startBeat.ToString();
        startBeat++;
        if (startBeat > beatThreshold)
        {
            startBeat = 1;
        }
    }
}

public class BeatDisplayEvent
{

}
