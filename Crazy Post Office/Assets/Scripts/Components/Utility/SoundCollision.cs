using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollision : MonoBehaviour
{
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.isTrigger) return;
        
        Debug.Log($"Speed is {rigidbody.velocity.magnitude}");
        
        SoundManager.PlayRandomClip(SoundType.Bounce, (float) Math.Pow(rigidbody.velocity.magnitude / 60f, 2));
    }
}
