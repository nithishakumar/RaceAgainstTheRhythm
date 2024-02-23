using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RevealText : MonoBehaviour
{
    public string text = "Use arrow keys to move to a  Safe Zone tile by the fourth beat.";
    TextMeshProUGUI textMeshProUGUI;
    AudioClip typingSfx;
    public void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        typingSfx = ResourceLoader.GetAudioClip("blipSfx");
        StartCoroutine(RevealTextRoutine());
    }
    public IEnumerator RevealTextRoutine()
    {
        for (int i = 1; i < text.Length + 1; i++)
        {
            AudioSource.PlayClipAtPoint(typingSfx, Camera.main.transform.position);
            textMeshProUGUI.text = text.Substring(0, i);
            yield return new WaitForSeconds(0.05f);
        }

        textMeshProUGUI.text = text;
    }
}
