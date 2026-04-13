using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttackTest : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private List<BaseAttack> baseAttacks;

    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.enabled = true;
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