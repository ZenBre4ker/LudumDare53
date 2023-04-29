using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Trigger springTrigger;
    private Vector3 resetMovePosition;
    private Vector3 finalMovePosition;
    private Vector3 globalMoveDelta;

    private float OnTriggerPushPosition = 0f;

    private float startPushTime;

    private float pushDuration = 2f;

    private bool startPush = false;
    // Start is called before the first frame update
    void Start()
    {
        startPushTime = Time.time;
        resetMovePosition = transform.position;

        transform.localPosition += new Vector3(0, 2f, 0);;
        finalMovePosition = transform.position;
        globalMoveDelta = finalMovePosition - resetMovePosition;
        
        transform.position = resetMovePosition;

        springTrigger.objectGotTriggered += gotTriggered;
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
        float timeDelta = Time.time - startPushTime;
        if (startPush)
        {
            OnTriggerPushPosition += timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = resetMovePosition + globalMoveDelta * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
        }
        else
        {
            OnTriggerPushPosition -= timeDelta / pushDuration;
            OnTriggerPushPosition = Math.Clamp(OnTriggerPushPosition,0,1);
            Vector3 moveToPosition = resetMovePosition + globalMoveDelta * OnTriggerPushPosition ;
            gameObject.GetComponent<Rigidbody>().MovePosition(moveToPosition);
            //gameObject.transform.position = resetMovePosition;
        }
    }
}
