using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private AttackEnergy attackEnergy;
    [SerializeField] private AttackSystem attackSystem;

    private void OnEnable()
    {
        if (attackEnergy == null) return;
        attackEnergy.OnAttackTriggered += HandleAttackTriggered;
        attackEnergy.OnDoubleDamageTriggered += HandleDoubleDamageTriggered;
    }

    private void OnDisable()
    {
        if (attackEnergy == null) return;
        attackEnergy.OnAttackTriggered -= HandleAttackTriggered;
        attackEnergy.OnDoubleDamageTriggered -= HandleDoubleDamageTriggered;
    }

    private void HandleDoubleDamageTriggered(float multiplier)
    {
        if (attackSystem == null) return;
        attackSystem.HandleDoubleDamageEffect(multiplier);
    }

    private void HandleAttackTriggered(BaseAttack attack)
    {
        if (attackSystem == null || attack == null) return;
        attackSystem.QueueAttack(attack);
    }
}