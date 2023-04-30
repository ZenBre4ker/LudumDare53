using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public GameObject startButton;

    public GameObject stopButton;

    public GameObject testButton;

    public GameObject pauseButton;
    public GameObject toggle3DButton;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        EnableAllButtons(false);
        
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.buildIndex != 0 && scene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                EnableAllButtons(true);
                IsToggled3D();
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
        stopButton.SetActive(enable);
        testButton.SetActive(enable);
        pauseButton.SetActive(enable);
        toggle3DButton.SetActive(enable);
    }

    public void EnableRun(bool enable)
    {
        startButton.SetActive(!enable);
        stopButton.SetActive(enable);
        testButton.SetActive(!enable);
        pauseButton.SetActive(enable);
    }

    public void StartPressed()
    {
        EnableRun(true);
        isPaused = false;
        
        LevelManager.StartLevel();
    }

    public void StopPressed()
    {
        EnableRun(false);
        isPaused = false;
        
        LevelManager.StopLevel();
    }

    public void PausePressed()
    {
        isPaused = !isPaused;
        LevelManager.EnablePauseLevel(isPaused);
    }

    public void TestPressed()
    {
        EnableRun(true);
        isPaused = false;
        LevelManager.TestLevel();
    }

    public void DoToggle3D(bool enabled)
    {
        toggle3DButton.GetComponent<Toggle>().isOn = enabled;
        IsToggled3D(enabled);
    }
    
    public void IsToggled3D(bool youAreShit = true)
    {
        PlayerManager.Singleton.ChangeTo3D(toggle3DButton.GetComponent<Toggle>().isOn);
    }
}
