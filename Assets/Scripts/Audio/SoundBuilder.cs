using UnityEngine;

public class SoundBuilder
{
    AudioManager manager;
    AudioData audioData;

    public SoundBuilder(AudioManager manager)
    {
        this.manager = manager;
    }

    public SoundBuilder WithAudioData(AudioData data)
    {
        this.audioData = data;
        return this;
    }

    public void Play()
    {
        AudioPlayer audioPlayer = manager.Get();
        audioPlayer.Init(audioData);
        audioPlayer.transform.SetParent(AudioManager.Instance.transform);

        audioPlayer.Play();
    }
}
