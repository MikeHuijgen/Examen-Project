using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioPool : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSourcePrefab;
    [SerializeField] private int initialSize = 30;
    [SerializeField] private Transform parent;

    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>();
    
    public void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var source = CreateNewAudioSource();
            _pool.Enqueue(source);
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        var source = Instantiate(AudioSourcePrefab, parent);
        source.gameObject.SetActive(false);
        return source;
    }

    public AudioSource GetAudioSource()
    {
        if (_pool.Count == 0) _pool.Enqueue(CreateNewAudioSource());

        var source = _pool.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    public void ReturnAudioSource(AudioSource source)
    {
        source.gameObject.SetActive(false);
        _pool.Enqueue(source);
    }
}
