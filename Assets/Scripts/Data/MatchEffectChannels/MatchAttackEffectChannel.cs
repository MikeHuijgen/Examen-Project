using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchAttackEffectChannel", menuName = "Scriptable Objects/MatchAttackEffectChannel")]
public class MatchAttackEffectChannel : ScriptableObject
{
    public event Action<BaseAttack> OnEventRaised;

    public void Raise(BaseAttack attack) => OnEventRaised?.Invoke(attack);
}
