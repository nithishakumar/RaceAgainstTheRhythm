using TMPro;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    Subscription<UpdateScoreEvent> updateScoreEventSub;
    Subscription<DisplayFinalScoreEvent> displayFinalScoreEventSub;
    void Start()
    {
        updateScoreEventSub = EventBus.Subscribe<UpdateScoreEvent>(_OnScoreUpdated);
        displayFinalScoreEventSub = EventBus.Subscribe<DisplayFinalScoreEvent>(_OnVictory);
    }

    void _OnScoreUpdated(UpdateScoreEvent e)
    {
        TextMeshProUGUI textComp = GetComponent<TextMeshProUGUI>();
        if (e.newScore < 0) e.newScore = 0;
        textComp.text = e.newScore.ToString() + "%";
        if(e.newScore >= 60)
        {
            // Set to green
            textComp.color = new Color(121f / 255f, 213f / 255f, 77f / 255f, 1f);
        }
        else if (e.newScore >= 50)
        {
            // Set to yellow
            textComp.color = new Color(253f / 255f, 189f / 255f, 26f / 255f, 1f);
        }
        else
        {
            // Set to red
            textComp.color = new Color(253f / 255f, 27f / 255f, 50f / 255f, 1f);
        }
        if(e.newScore < 50)
        {
            EventBus.Publish<DeathEvent>(new DeathEvent());
        }
    }

    void _OnVictory(DisplayFinalScoreEvent e)
    {
        StopAllCoroutines();
        GameObject Victory = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        Victory.SetActive(true);
        GameObject VictoryText = Victory.transform.GetChild(0).gameObject;
        GameObject accuracyText = VictoryText.transform.GetChild(0).gameObject;
        accuracyText.GetComponent<TextMeshProUGUI>().text = "Your accuracy: " + e.accuracy.ToString() + "%";
        accuracyText.SetActive(true);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(updateScoreEventSub);
        EventBus.Unsubscribe(displayFinalScoreEventSub);
    }
}

public class UpdateScoreEvent
{
    public int newScore = 0;
    public UpdateScoreEvent(int _newScore) { newScore = _newScore; }
}

public class DisplayFinalScoreEvent
{
    public int accuracy = 0;
    public  DisplayFinalScoreEvent(int _accuracy) { accuracy = _accuracy; }
}

