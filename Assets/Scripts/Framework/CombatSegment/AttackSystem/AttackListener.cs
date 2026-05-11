using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private MatchAttackEffectChannel matchAttackEffectChannel;

    private void OnEnable() => matchAttackEffectChannel.OnEventRaised += HandleMatchDestroyed;
    private void OnDisable() => matchAttackEffectChannel.OnEventRaised -= HandleMatchDestroyed;
    private void HandleMatchDestroyed(BaseAttack attack) => attackSystem.TriggerAttack(attack);
}
