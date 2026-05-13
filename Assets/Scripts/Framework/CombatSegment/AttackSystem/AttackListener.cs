using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private AttackEnergy attackEnergy;
    [SerializeField] private AttackSystem attackSystem;

    private void OnEnable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnAttackTriggered += HandleAttackTriggered;
            attackEnergy.OnDoubleDamageTriggered += HandleDoubleDamageTriggered;
        }
    }

    private void OnDisable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnAttackTriggered -= HandleAttackTriggered;
            attackEnergy.OnDoubleDamageTriggered -= HandleDoubleDamageTriggered;
        }
    }

    private void HandleAttackTriggered(BaseAttack attack)
    {
        if (attackSystem == null || attack == null) return;
        attackSystem.QueueAttack(attack);
    }

    private void HandleDoubleDamageTriggered(float multiplier)
    {
        Debug.Log("double damage fire from listener");
    }
}