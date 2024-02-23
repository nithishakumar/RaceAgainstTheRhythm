using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpacebarEvent : MonoBehaviour
{
    int canvasChildIdx = 0;
    int numChildren;
    GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        numChildren = canvas.transform.childCount - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(canvasChildIdx + 1 < numChildren)
            {
                Debug.Log("hello");
                canvas.transform.GetChild(canvasChildIdx).gameObject.SetActive(false);
                canvasChildIdx++;
                canvas.transform.GetChild(canvasChildIdx).gameObject.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene("Level 1");
            }
            
        }
        
    }
}
