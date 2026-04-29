using UnityEngine;

[CreateAssetMenu(fileName = "MatchAttackEffect", menuName = "Scriptable Objects/MatchAttackEffect")]
public class MatchAttackEffect : BaseMatchEffect
{
    public BaseAttack Attack;
    public MatchAttackEffectChannel effectChannel;

    public override void ActivateEffect() => effectChannel.Raise(Attack);
}
