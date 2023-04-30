using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    public Transform parentTransform;
    
    private Vector3 resetMovePosition;
    private Vector3 localMoveDelta;

    public bool shouldMove = true;
    public bool isPaused = false;

    public float moveSpeed = 1f;

    public float startVolume = -1f;

    private AudioSource myAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        resetMovePosition = transform.localPosition;

        localMoveDelta = new Vector3(0.4f/transform.lossyScale.x, 0, 0);
        
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

        if (TryGetComponent(out AudioSource audioSource))
        {
            myAudioSource = audioSource;
            startVolume = myAudioSource.volume;
            myAudioSource.pitch = moveSpeed;
        }
        
        SoundManager.onSoundChanged += volume =>
        {
            
            if (myAudioSource == null && !TryGetComponent(out myAudioSource)) return;
            
            if (startVolume < 0)
            {
                startVolume = myAudioSource.volume;
            }

            myAudioSource.volume = startVolume * volume;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myAudioSource.mute = isPaused || !shouldMove;
        if (!shouldMove || isPaused) return;

        Vector3 globalResetPosition = parentTransform.position + transform.TransformDirection(resetMovePosition);
        gameObject.GetComponent<Rigidbody>().position = globalResetPosition;
        gameObject.GetComponent<Rigidbody>().MovePosition(globalResetPosition + transform.TransformDirection(localMoveDelta) * moveSpeed);
    }
}
