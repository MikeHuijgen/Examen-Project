using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchDoubleDamageEffectChannel", menuName = "Scriptable Objects/MatchDoubleDamageEffectChannel")]
public class MatchDoubleDamageEffectChannel : ScriptableObject
{
    public event Action<float> OnEventRaised;

    public void Raise(float multiplier)
    {
        OnEventRaised?.Invoke(multiplier);
        Debug.Log($"Activated double damage with the multiplier of: {multiplier}");
    }
}
