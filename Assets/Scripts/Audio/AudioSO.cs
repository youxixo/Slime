using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioDataConfig
{
    public List<AudioData> ConfList = new List<AudioData>();
}

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "AudioDataSO", order = 0)]
public class AudioSO : ScriptableObject
{
    public AudioDataConfig Conf;
}