using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndReload());
    }

    IEnumerator WaitAndReload()
    {
        yield return new WaitForSeconds(60);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
