using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class AudioPlayer : MonoBehaviour
{
    public AudioData Data;
    AudioSource audioSource;
    Coroutine playingCoroutine;

    private void Awake()
    {
        audioSource = gameObject.GetOrAddComponent<AudioSource>();
    }

    public void Play()
    {
        if(playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }
        audioSource.Play();
        if (!audioSource.loop)
        {
            playingCoroutine = StartCoroutine(Waitforsound());
        }
    }

    private IEnumerator Waitforsound()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        AudioManager.Instance.ReturnToPool(this);
    }

    public void Stop()
    {
        if(playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }
        audioSource.Stop();
        AudioManager.Instance.ReturnToPool(this);
    }

    public void Init(AudioData data)
    {
        Data = data;
        audioSource.clip = Resources.Load<AudioClip>(data.clipPath);
        audioSource.loop = data.loop;
        audioSource.outputAudioMixerGroup = AudioManager.Instance.audioMixer.FindMatchingGroups(data.audioMixer)[0];
        audioSource.playOnAwake = data.playOnAwake;
    }
}
