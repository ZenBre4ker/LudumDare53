using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public void Start()
    {
        int availablePackages = 0;
        int receivedPackages = 0;
        int sentPackages = 0;
        float neededTime = 0;
        
        for(int i = 1; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            LevelManager.LevelAchievement levelAchievement = LevelManager.levelAchievements[i];
            availablePackages += levelAchievement.availablePackages;
            receivedPackages += levelAchievement.receivedPackages;
            sentPackages += levelAchievement.sentPackages;
            neededTime += levelAchievement.levelTime;
        }

        resultText.text = $"You have {receivedPackages} of {availablePackages} Packages successfully D.E.S.T.R.O.Y-ed.\n Time: {neededTime} seconds ";
    }

    public void RestartGame()
    {
        LevelManager.StartFirstScene();
    }
}
