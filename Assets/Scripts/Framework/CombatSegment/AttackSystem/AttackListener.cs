using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private ComboCounter ComboCounter;
    [SerializeField] private AttackEnergy attackEnergy;
    [SerializeField] private MatchAttackEffectChannel matchAttackEffectChannel;

    private void OnEnable() => matchAttackEffectChannel.OnEventRaised += HandleMatchDestroyed;
    private void OnDisable() => matchAttackEffectChannel.OnEventRaised -= HandleMatchDestroyed;

    private void HandleMatchDestroyed(BaseAttack attack)
    {
        ComboCounter.OnSuccessfulHit();
        attackEnergy.OnMatch(attack);
    }
}
