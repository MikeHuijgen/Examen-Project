using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject
{
    public string Identifier;

    public AudioClip AudioClip;

    [Range(0f, 1f)] public float Volume = 1f;
    public float Pitch = 1f;
    public bool Loop = false;
}