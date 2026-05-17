using UnityEngine;

[CreateAssetMenu(fileName = "MatchDubbleDamageEffect", menuName = "Scriptable Objects/MatchDubbleDamageEffect")]
public class MatchDoubleDamageEffect : BaseMatchEffect
{
    public float Multiplier = 1.5f;
    public MatchDoubleDamageEffectChannel effectChannel;
    public override void ActivateEffect() => effectChannel.Raise(Multiplier);
}
