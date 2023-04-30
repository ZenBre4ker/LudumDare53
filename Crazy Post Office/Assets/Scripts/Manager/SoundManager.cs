using System;
using System.Collections;
using System.Collections.Generic;
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

public class SoundManager : MonoBehaviour
{
    private static SoundManager Singleton;
    
    public List<AudioClip> PackageSentClips;
    public List<AudioClip> PackageReceivedClips;
    public List<AudioClip> BounceClips;
    public List<AudioClip> PistonClips;
    public List<AudioClip> LevelOverClips;

    private AudioSource[] audioSources;

    private int maxAudioSources = 100;

    private int nextAudioSource = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (Singleton == null) return;
        Singleton = this;

        audioSources = new AudioSource[maxAudioSources];

        for (int i = 0; i < maxAudioSources; i++)
        {
            audioSources[i] = new AudioSource();
        }
    }

    void PlayRandomClip(SoundType type)
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

        AudioSource source = audioSources[nextAudioSource++];
        nextAudioSource %= maxAudioSources;
        source.clip = clips[Random.Range(0, clips.Count)];
        source.Play();
    }
}
