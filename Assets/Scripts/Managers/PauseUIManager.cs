using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUIManager : MonoBehaviour
{

    public GameObject PauseCanvas;
    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        PauseCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                PauseCanvas.SetActive(true);
                paused = true;
            }
            else
            {
                PauseCanvas.SetActive(false);
                paused = false;
            }
        }
    }
}
