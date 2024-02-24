using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    public void Restart()
    {
        RhythmEventManager.wasSceneReloaded = true;
        // Set the game over menu to be inactive
        GameObject.Find("Canvas").transform.GetChild(1).transform.gameObject.SetActive(false);
        // Set the health bar to be full
        EventBus.Publish<ResetHealthEvent>(new ResetHealthEvent());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
