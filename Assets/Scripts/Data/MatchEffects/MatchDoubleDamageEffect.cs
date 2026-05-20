using UnityEngine;

[CreateAssetMenu(fileName = "MatchDubbleDamageEffect", menuName = "Scriptable Objects/MatchDubbleDamageEffect")]
public class MatchDoubleDamageEffect : BaseMatchEffect
{
    public float Multiplier = 2f;
    public MatchDoubleDamageEffectChannel EffectChannel;
    
    public override void ActivateEffect() => EffectChannel.Raise(Multiplier);
}
