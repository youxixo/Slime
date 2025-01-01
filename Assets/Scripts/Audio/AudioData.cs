using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioData
{
    //public AudioClip clip;
    //public AudioMixerGroup mixerGroup;
    public string name;
    public string clipPath;
    public string audioMixer;
    public bool loop;
    public bool playOnAwake;
}
