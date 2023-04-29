using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Trigger springTrigger;
    
    public Transform parentTransform;
    
    private Vector3 resetMovePosition;
    private Vector3 localMoveDelta;

    private float OnTriggerPushPosition = 0f;

    private float startPushTime;

    private float pushDuration = 2f;

    private bool startPush = false;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        startPushTime = Time.time;
        resetMovePosition = transform.localPosition;

        localMoveDelta = new Vector3(0, 2f, 0);;

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
        if (info.isExited)
        {
            //gameObject.GetComponent<Rigidbody>().MovePosition(finalMovePosition);
            startPush = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPaused) return;
        
        float timeDelta = Time.time - startPushTime;
        Vector3 globalResetPosition = parentTransform.position + transform.TransformDirection(resetMovePosition);
        if (startPush)
        {
            OnTriggerPushPosition += timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = globalResetPosition + transform.TransformDirection(localMoveDelta) * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
        }
        else
        {
            OnTriggerPushPosition -= timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = globalResetPosition + transform.TransformDirection(localMoveDelta) * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
            //gameObject.transform.position = resetMovePosition;
        }
    }
}
