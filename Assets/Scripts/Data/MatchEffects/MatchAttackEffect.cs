using UnityEngine;

[CreateAssetMenu(fileName = "MatchAttackEffect", menuName = "Scriptable Objects/MatchAttackEffect")]
public class MatchAttackEffect : BaseMatchEffect
{
    public BaseAttack Attack;
    public MatchAttackEffectChannel EffectChannel;

    public override void ActivateEffect() => EffectChannel.Raise(Attack);
}
