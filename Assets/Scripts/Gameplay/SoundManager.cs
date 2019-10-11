using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> audioClips;
    private static SoundManager instance;
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        source = GetComponent<AudioSource>();
        foreach (AudioClip a in audioClips)
        {
            _audioClips.Add(a.name, a);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void PlayOnce(string name)
    {
        AudioClip clip = instance._audioClips[name];
        instance.source.PlayOneShot(clip);
    }

    public static void PlayOnce(AudioClip clip)
    {
        instance.source.PlayOneShot(clip);
    }

    public static void LoopPlay(string name)
    {
        AudioClip clip = instance._audioClips[name];
        instance.source.clip = clip;
        instance.source.Play();
    }

    public static void LoopPlay(AudioClip clip)
    {
        instance.source.clip = clip;
        instance.source.Play();
    }

    public static void Stop()
    {
        instance.source.Stop();
    }

    public static void Resume()
    {
        instance.source.Play();
    }
}
