using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class PackageManager : MonoBehaviour
{
    public enum levelOverReason
    {
        ReceivedAllPackages,
        AllPackagesStill,
        TimeOver
    }
    public delegate void LevelOverDelegate(LevelManager.LevelAchievement levelAchievement, levelOverReason reason);
    
    public GameObject packagePrefab;
    public float stillTimeCheckInterval = 0.1f;
    public float velocityNearZero = (float) Math.Pow(10, -18);
    
    public int sentPackages = 0;
    public int receivedPackages = 0;

    public static LevelOverDelegate notifyOnLevelOver; 
    
    private GameObject[] goals;
    private GameObject[] spawns;
    private Dictionary<int, GameObject> sentPackageObjects;
    private LevelIdentification levelSetup;

    private bool[] spawnClear;
    private float[] lastSpawnClearTime;

    private float lastStillCheckTime;

    [SerializeField]
    private int stillPackages = 0;

    private float levelStartTime;
    private float pauseStartTime;
    private bool levelActive = false;
    private bool isTestRun = false;

    void Start()
    {
        sentPackageObjects = new Dictionary<int, GameObject>();
        
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.buildIndex != 0 && scene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                SetupLevel();
            }
        };

        LevelManager.onLevelStarted += StartLevel;
        LevelManager.onLevelTested += () =>
        {
            ResetLevel();
            isTestRun = true;
            TrySpawnPackages();
        };
        LevelManager.onLevelStopped += ResetLevel;
        LevelManager.onLevelPaused += () =>
        {
            EnablePauseLevel(true);
        };
        LevelManager.onLevelUnpaused += () =>
        {
            EnablePauseLevel(false);
        };
    }
    
    // Start is called before the first frame update
    public void StartLevel()
    {
        ResetLevel();
        
        levelActive = true;
    }

    private void SetupLevel()
    {
        goals = GameObject.FindGameObjectsWithTag("PackageGoal");
        
        for (int i =0; i <goals.Length; i++)
        {
            GameObject goal = goals[i];
            int number = i;
            goal.GetComponent<Trigger>().objectGotTriggered += info =>
            {
                goalGotTriggered(info, number);
            };
        }
        
        spawns = GameObject.FindGameObjectsWithTag("PackageSpawn");
        spawnClear = new bool [spawns.Length];
        lastSpawnClearTime = new float [spawns.Length];

        for (int i =0; i <spawns.Length; i++)
        {
            GameObject spawn = spawns[i];
            int number = i;
            spawn.GetComponent<Trigger>().objectGotTriggered += info =>
            {
                spawnGotTriggered(info, number);
            };
        }
        
        levelSetup = GameObject.FindWithTag("LevelSetup").GetComponent<LevelIdentification>();
    }
    
    private void ResetLevel()
    {
        float currentTime = Time.time;
        levelStartTime = currentTime;
        lastStillCheckTime = currentTime;
        
        sentPackages = 0;
        receivedPackages = 0;
        
        isTestRun = false;
        levelActive = false;

        for (int i = 0; i < spawns.Length; i++)
        {
            lastSpawnClearTime[i] = 0;
            spawnClear[i] = true;
        }

        foreach (GameObject package in sentPackageObjects.Values)
        {
            Destroy(package);
        }

        sentPackageObjects = new Dictionary<int, GameObject>();
    }
    
    private void FixedUpdate()
    {
        if (!levelActive || isTestRun) return;
        
        float currentTime = Time.time;
        
        if (!levelSetup.infiniteSpawns && currentTime - levelStartTime > levelSetup.timeForLevelCompletion)
        {
            LevelOver(levelOverReason.TimeOver);
            return;
        }

        TrySpawnPackages();

        if (!levelSetup.infiniteSpawns && levelSetup.checkStillTime && currentTime - lastStillCheckTime >= stillTimeCheckInterval)
        {
            stillPackages = 0;
            lastStillCheckTime = currentTime;
            foreach (GameObject package in sentPackageObjects.Values)
            {
                Rigidbody rb = package.GetComponent<Rigidbody>();
                float velocity = rb.velocity.magnitude + rb.angularVelocity.magnitude;

                PacketIdentification packetId = package.GetComponent<PacketIdentification>();
                if (velocity < velocityNearZero)
                {
                    if (packetId.isStill && currentTime - packetId.stillSince >= levelSetup.allowedStillTime)
                    {
                        stillPackages += 1;
                    }
                    else if (!packetId.isStill)
                    {
                        packetId.isStill = true;
                        packetId.stillSince = currentTime;
                    }
                }
                else
                {
                    packetId.isStill = false;
                }
            }

            if (stillPackages > 0 && stillPackages >= sentPackages - receivedPackages)
            {
                LevelOver(levelOverReason.AllPackagesStill);
            }
        }
        
    }

    private void TrySpawnPackages()
    {
        int maxPackages = isTestRun ? spawnClear.Length : levelSetup.numberOfPackages;
        
        for (int i = 0; i < spawns.Length; i++)
        {
            if (!levelSetup.infiniteSpawns && sentPackages >= maxPackages) break;
            
            float deltaClearTime = Time.time - lastSpawnClearTime[i];
            if (spawnClear[i] && deltaClearTime >= levelSetup.timeBetweenPackages )
            {
                spawnClear[i] = false;
                GameObject newPackage = Instantiate(packagePrefab, spawns[i].transform.position, spawns[i].transform.rotation);
                newPackage.GetComponent<PacketIdentification>().packetNumber = sentPackages;
                sentPackageObjects.Add(sentPackages, newPackage);
                sentPackages += 1;
            }
        }
    }

    private void goalGotTriggered(Trigger.TriggerInfo info, int goalNumber)
    {
        if (!info.detectedCollider.gameObject.CompareTag("Package") || !info.isEntered) return;
        receivedPackages += 1;
        sentPackageObjects.Remove(info.detectedCollider.gameObject.GetComponent<PacketIdentification>().packetNumber);
        Destroy(info.detectedCollider.gameObject);

        if (!isTestRun && !levelSetup.infiniteSpawns && receivedPackages == levelSetup.numberOfPackages)
        {
            LevelOver(levelOverReason.ReceivedAllPackages);
        }
    }
    
    private void spawnGotTriggered(Trigger.TriggerInfo info, int spawnNumber)
    {
        if (!info.detectedCollider.gameObject.CompareTag("Package")) return;

        if (info.isExited)
        {
            spawnClear[spawnNumber] = true;
        }

        if (info.isStaying)
        {
            lastSpawnClearTime[spawnNumber] = Time.time;
        }
    }

    private void EnablePauseLevel(bool enable)
    {
        levelActive = !enable;
        
        foreach (GameObject package in sentPackageObjects.Values)
        {
            Rigidbody rb =package.GetComponent<Rigidbody>();
            rb.isKinematic = enable;
            PacketIdentification id = package.GetComponent<PacketIdentification>();
            if (enable)
            {
                id.lastMovementSpeed = rb.velocity;
                id.lastRotationSpeed = rb.angularVelocity;
            }
            else
            {
                rb.velocity = id.lastMovementSpeed;
                rb.angularVelocity = id.lastRotationSpeed;
            }
        }

        if (enable)
        {
            pauseStartTime = Time.time;
        }
        else
        {
            levelStartTime += Time.time - pauseStartTime;
        }
    }
    
    private void LevelOver(levelOverReason reason)
    {
        EnablePauseLevel(true);

        notifyOnLevelOver?.Invoke(new LevelManager.LevelAchievement()
        {
            availablePackages = levelSetup.numberOfPackages,
            sentPackages = sentPackages,
            receivedPackages = receivedPackages,
            levelTime = Time.time - levelStartTime
        }, reason);
        
        Debug.Log($"Level is over because: {reason}");
    }
}
