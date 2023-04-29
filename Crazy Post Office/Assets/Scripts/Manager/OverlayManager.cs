using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public GameObject startButton;

    public GameObject stopButton;

    public GameObject testButton;

    public GameObject pauseButton;
    // Start is called before the first frame update
    void Start()
    {
        EnableAllButtons(false);
        
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.buildIndex != 0 && scene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                EnableAllButtons(true);
            }
            else
            {
                EnableAllButtons(false);
            }
        };
    }

    public void EnableAllButtons(bool enable)
    {
        startButton.SetActive(enable);
        stopButton.SetActive(!enable);
        testButton.SetActive(enable);
        pauseButton.SetActive(enable);
    }

    public void EnableRun(bool enable)
    {
        startButton.SetActive(!enable);
        stopButton.SetActive(enable);
        testButton.SetActive(!enable);
        pauseButton.SetActive(enable);
    }

    public void EnableTestRun(bool enable)
    {
        EnableRun(enable);
    }

    public void StartPressed()
    {
        EnableRun(true);
    }

    public void StopPressed()
    {
        EnableRun(false);
    }

    public void PausePressed()
    {
        
    }

    public void TestPressed()
    {
        EnableTestRun();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
