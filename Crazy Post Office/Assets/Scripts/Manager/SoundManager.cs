using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SoundType
{
    PackageSent,
    PackageReceived,
    Bounce,
    Piston,
    LevelOver
}

public delegate void soundChanged(float newVolume);

public class SoundManager : MonoBehaviour
{
    private static SoundManager Singleton;
    
    public List<AudioClip> PackageSentClips;
    public List<AudioClip> PackageReceivedClips;
    public List<AudioClip> BounceClips;
    public List<AudioClip> PistonClips;
    public List<AudioClip> LevelOverClips;

    public AudioSource audioSource;

    public static soundChanged onSoundChanged;
    // Start is called before the first frame update
    void Start()
    {
        if (Singleton != null) return;
        Singleton = this;
    }

    public static void PlayRandomClip(SoundType type, float volume = 0.3f)
    {
        List<AudioClip> clips;
        switch (type)
        {
            case SoundType.PackageSent:
                clips = Singleton.PackageSentClips;
                break;
            case SoundType.PackageReceived:
                clips = Singleton.PackageReceivedClips;
                break;
            case SoundType.Bounce:
                clips = Singleton.BounceClips;
                break;
            case SoundType.Piston:
                clips = Singleton.PistonClips;
                break;
            case SoundType.LevelOver:
                clips = Singleton.LevelOverClips;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        Singleton.audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volume);
    }

    public static void ChangeSoundVolume(float volume)
    {
        Singleton.audioSource.volume = volume;
        onSoundChanged?.Invoke(volume);
    }
}
