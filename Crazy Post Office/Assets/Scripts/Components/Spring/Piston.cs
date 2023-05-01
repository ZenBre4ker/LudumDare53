using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Piston : MonoBehaviour
{
    public Trigger springTrigger;
    
    public Transform parentTransform;
    
    private Vector3 resetMovePosition;
    private Vector3 localMoveDelta;

    private float OnTriggerPushPosition = 0f;

    private float startPushTime;

    private float minPushDuration = 0.7f;
    private float maxPushDuration = 0.75f;
    private float pushDuration;

    private bool startPush = false;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        startPushTime = Time.time;
        resetMovePosition = transform.localPosition;

        localMoveDelta = new Vector3(0, 0, 2f);;

        springTrigger.objectGotTriggered += gotTriggered;

        LevelManager.onLevelPaused += () =>
        {
            isPaused = true;
        };
        LevelManager.onLevelUnpaused += () =>
        {
            isPaused = false;
        };
        LevelManager.onLevelStarted += () =>
        {
            isPaused = false;
        };
        LevelManager.onLevelStopped += () =>
        {
            OnTriggerPushPosition = 0f;
            isPaused = true;
        };
        LevelManager.onLevelTested += () =>
        {
            isPaused = false;
        };
    }

    private void gotTriggered(Trigger.TriggerInfo info)
    {
        if (!info.detectedCollider.gameObject.CompareTag("Package")) return;
        
        if (info.isEntered)
        {
            //gameObject.GetComponent<Rigidbody>().MovePosition(finalMovePosition);
            startPush = true;
            startPushTime = Time.time;
        }

        if (info.isStaying && OnTriggerPushPosition <= 0)
        {
            startPush = true;
            startPushTime = Time.time;
        }
        
        if (info.isExited)
        {
            //gameObject.GetComponent<Rigidbody>().MovePosition(finalMovePosition);
            //startPush = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPaused) return;

        if (startPush && OnTriggerPushPosition <= 0)
        {
            pushDuration = Random.Range(minPushDuration, maxPushDuration);
            SoundManager.PlayRandomClip(SoundType.Piston,  1 / 30f / pushDuration);
        }
        
        float timeDelta = Time.time - startPushTime;
        Vector3 globalResetPosition = parentTransform.position + parentTransform.TransformVector(resetMovePosition);
        if (startPush)
        {
            OnTriggerPushPosition += timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = globalResetPosition + parentTransform.TransformDirection(localMoveDelta) * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
            if (OnTriggerPushPosition >= 1)
            {
                startPush = false;
            }
        }
        else
        {
            OnTriggerPushPosition -= timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = globalResetPosition + parentTransform.TransformDirection(localMoveDelta) * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
            //gameObject.transform.position = resetMovePosition;
        }
    }
}
