using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

[DefaultExecutionOrder(-99)]
public class AudioManager : MonoBehaviour
{
    public AudioDataSO audioDataSo;
    public AudioMixer audioMixer;
    private Transform _transform;

    private IObjectPool<AudioPlayer> audioDataPool;
    private List<AudioPlayer> activeAudio = new();
    public readonly Dictionary<AudioData, int> Counts = new();
    public AudioPlayer _audioPrefab;
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxCapacity = 100;
    [SerializeField] int maxSoundInstance = 30;

    public SoundBuilder CreateSound() => new  SoundBuilder(this);

    private void OnTakeFromPool(AudioPlayer audioData)
    {
        audioData.gameObject.SetActive(true);
        activeAudio.Add(audioData);
    }

    private AudioPlayer CreateAudioData()
    {
        var audioData = Instantiate(_audioPrefab);
        audioData.gameObject.SetActive(false);
        return audioData;
    }

    private void InitPool()
    {
        audioDataPool = new ObjectPool<AudioPlayer>(
            CreateAudioData,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPool,
            collectionCheck,
            defaultCapacity,
            maxCapacity
            );
    }

    private void OnReturnedToPool(AudioPlayer audioData)
    {
        if(Counts.TryGetValue(audioData.Data, out var count))
        {
            Counts[audioData.Data] -= count > 0 ? 1 : 0;
        }

        audioData.gameObject.SetActive(false);
        activeAudio.Remove(audioData);
    }

    private void OnDestroyPool(AudioPlayer audioData)
    {
        Destroy(audioData.gameObject);
    }

    public AudioPlayer Get()
    {
        return audioDataPool.Get();
    }

    public void ReturnToPool(AudioPlayer audioData)
    {
        audioDataPool.Release(audioData);
    }

    private static AudioManager _instance = null;

    private List<AudioSource> _playingAudioSources = new List<AudioSource>();
    public static AudioManager Instance
    {
        get { return _instance; }
    }
    private ObjectPool<GameObject> pool;

    private void Awake()
    {
        _instance = this;
        _transform = transform;
        if (transform.parent)
        {
            DontDestroyOnLoad(transform.parent);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        InitPool();
    }
}