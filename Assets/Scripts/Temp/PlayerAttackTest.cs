using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttackTest : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private List<BaseAttack> baseAttacks;

    public void TriggerAttack(int index)
    {
        if (index < 0 || index >= baseAttacks.Count)
            return;

        var attack = baseAttacks[index];
        attackSystem.TriggerAttack(attack);
    }
}