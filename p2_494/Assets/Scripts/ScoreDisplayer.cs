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
        TextMeshProUGUI textComp = GetComponent<TextMeshProUGUI>();
        if (e.new_score < 0) e.new_score = 0;
        textComp.text = e.new_score.ToString() + "%";
        if(e.new_score >= 60)
        {
            // Set to green
            textComp.color = new Color(121f / 255f, 213f / 255f, 77f / 255f, 1f);
        }
        else if (e.new_score >= 25)
        {
            // Set to yellow
            textComp.color = new Color(253f / 255f, 189f / 255f, 26f / 255f, 1f);
        }
        else
        {
            // Set to red
            textComp.color = new Color(253f / 255f, 27f / 255f, 50f / 255f, 1f);
        }
        if(e.new_score <= 10)
        {
            EventBus.Publish<DeathEvent>(new DeathEvent());
        }
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
