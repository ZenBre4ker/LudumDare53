using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public delegate void ObjectGotTriggeredDelegate(TriggerInfo info);
    
    public class TriggerInfo
    {
        public bool isEntered = false;
        public bool isExited = false;
        public bool isStaying = false;
        public Collider detectedCollider;
    }

    public ObjectGotTriggeredDelegate objectGotTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (objectGotTriggered == null) return;
        
        TriggerInfo info = new TriggerInfo
        {
            isEntered = true,
            detectedCollider = other
        };
        
        objectGotTriggered.Invoke(info);
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectGotTriggered == null) return;
        
        TriggerInfo info = new TriggerInfo
        {
            isExited = true,
            detectedCollider = other
        };
        
        objectGotTriggered.Invoke(info);
    }

    private void OnTriggerStay(Collider other)
    {
        if (objectGotTriggered == null) return;
        
        TriggerInfo info = new TriggerInfo
        {
            isStaying = true,
            detectedCollider = other
        };
        
        objectGotTriggered.Invoke(info);
    }
}
