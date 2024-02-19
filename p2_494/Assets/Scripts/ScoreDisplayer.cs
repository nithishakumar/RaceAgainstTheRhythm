using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    Subscription<UpdateScoreEvent> updateScoreEventSub;
    void Start()
    {
        updateScoreEventSub = EventBus.Subscribe<UpdateScoreEvent>(_OnScoreUpdated);
    }

    void _OnScoreUpdated(UpdateScoreEvent e)
    {
        GetComponent<TextMeshProUGUI>().text = e.new_score.ToString() + "%";
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(updateScoreEventSub);
    }
}

public class UpdateScoreEvent
{
    public int new_score = 0;
    public UpdateScoreEvent(int _new_score) { new_score = _new_score; }
}
