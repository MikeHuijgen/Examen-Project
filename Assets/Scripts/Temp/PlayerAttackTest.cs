using System.Collections.Generic;
using UnityEngine;
public class PlayerAttackTest : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private List<BaseAttack> baseAttacks;

    private void Update()
    {
        if (!attackSystem.IsIdle)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        TriggerAttack(Random.Range(0, baseAttacks.Count));
    }

    private void TriggerAttack(int index)
    {
        if (index < 0 || index >= baseAttacks.Count)
            return;

        var attack = baseAttacks[index];
        attackSystem.TriggerAttack(attack);
    }
}