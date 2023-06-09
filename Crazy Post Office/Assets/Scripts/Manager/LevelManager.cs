using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public class LevelAchievement
    {
        public int availablePackages = 0;
        public int sentPackages = -1;
        public int receivedPackages = -1;
        public float levelTime = Single.MaxValue;

        public bool AddBetterStats(LevelAchievement newAchievement)
        {
            availablePackages = newAchievement.availablePackages;
            int newReceivedPackages = newAchievement.receivedPackages;
            
            if ( newReceivedPackages > receivedPackages)
            {
                sentPackages = newAchievement.sentPackages;
                receivedPackages = newAchievement.receivedPackages;
                levelTime = newAchievement.levelTime;
                
                return true;
            } else if (newReceivedPackages == receivedPackages && newAchievement.levelTime < levelTime)
            {
                sentPackages = newAchievement.sentPackages;
                levelTime = newAchievement.levelTime;
                
                return true;
            }

            return false;
        }
    }
    
    private static int currentSceneIndex = 0;

    public static LevelAchievement[] levelAchievements;

    public static Action onLevelStarted;
    public static Action onLevelStopped;
    public static Action onLevelPaused;
    public static Action onLevelUnpaused;
    public static Action onLevelTested;

    public static void StartLevel()
    {
        onLevelStarted?.Invoke();
    }
    
    public static void StopLevel()
    {
        onLevelStopped?.Invoke();
    }
    
    public static void EnablePauseLevel(bool enable)
    {
        if (enable)
        {
            onLevelPaused?.Invoke();
        }
        else
        {
            onLevelUnpaused?.Invoke();
        }
    }
    
    public static void TestLevel()
    {
        onLevelTested?.Invoke();
    }
    
    private void Start()
    {
        levelAchievements = new LevelAchievement[SceneManager.sceneCountInBuildSettings];
        DontDestroyOnLoad(gameObject);
        PackageManager.notifyOnLevelOver += EndLevel;
    }

    public static void StartFirstScene()
    {
        currentSceneIndex = 0;
        
        LoadNextScene();
    }

    public static void LoadNextScene()
    {
        PlayerManager.onUserFire = null;
        SceneManager.LoadScene(++currentSceneIndex);
    }

    private void EndLevel(LevelAchievement newAchievement, PackageManager.levelOverReason reason)
    {
        //LevelAchievement levelAchievement = levelAchievements[currentSceneIndex];
        LevelAchievement levelAchievement = new LevelAchievement();
        levelAchievement.AddBetterStats(newAchievement);
        levelAchievements[currentSceneIndex] = levelAchievement;
        LoadNextScene();
    }
    
}
