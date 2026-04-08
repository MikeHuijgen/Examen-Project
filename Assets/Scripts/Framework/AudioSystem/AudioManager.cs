using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Library")]
    [SerializeField] private SoundData[] sounds;

    [Header("Audio Pool")]
    [SerializeField] private AudioPool audioPool;

    private Dictionary<string, SoundData> _soundLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioPool.InitializePool();
        BuildSoundLookup();
    }

    private void BuildSoundLookup()
    {
        _soundLookup = new Dictionary<string, SoundData>();

        foreach (SoundData sound in sounds) _soundLookup.Add(sound.Identifier, sound);
    }

    public void PlaySound(string soundIdentifier)
    {
        _soundLookup.TryGetValue(soundIdentifier, out SoundData sound);

        var source = audioPool.GetAudioSource();
        
        source.pitch = sound.Pitch;
        source.loop = sound.Loop;

        if (sound.Loop)
        {
            source.clip = sound.AudioClip;
            source.volume = sound.Volume;
            source.Play();
        }
        else
        {
            source.PlayOneShot(sound.AudioClip, sound.Volume);
            StartCoroutine(StopSoundOnFinish(source));
        }
    }
    
    public void StopSound(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.loop = false;
        source.pitch = 1f;
        
        audioPool.ReturnAudioSource(source);
    }
    private IEnumerator StopSoundOnFinish(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        StopSound(source);
    }
}