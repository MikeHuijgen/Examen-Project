using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Library")]
    [SerializeField] private SoundData[] sounds;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

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

        sfxSource.pitch = sound.Pitch;
        sfxSource.loop = sound.Loop;

        if (sound.Loop)
        {
            sfxSource.clip = sound.AudioClip;
            sfxSource.volume = sound.Volume;
            sfxSource.Play();
        }
        else
        {
            sfxSource.PlayOneShot(sound.AudioClip, sound.Volume);
        }
    }

    public void StopSfx()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
        sfxSource.loop = false;
        sfxSource.pitch = 1f;
    }

    public void PlayMusic(string soundIdentifier)
    {
        _soundLookup.TryGetValue(soundIdentifier, out SoundData sound);

        musicSource.clip = sound.AudioClip;
        musicSource.volume = sound.Volume;
        musicSource.pitch = sound.Pitch;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }
}