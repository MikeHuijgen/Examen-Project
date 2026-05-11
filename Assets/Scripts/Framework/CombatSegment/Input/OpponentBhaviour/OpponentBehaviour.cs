using System.Collections.Generic;
using UnityEngine;

public class OpponentBehaviour : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;

    [SerializeField] private List<OpponentAttack> opponentAttacks;
    [SerializeField] private List<GameObject> attackDirectionWarnings;

    [SerializeField] private float minAttackDelayTime;
    [SerializeField] private float maxAttackDelayTime;

    private CountdownTimer _idleTimer;
    private TimerManager _timer;

    private float _currentDelay;

    private void Start()
    {
        attackDirectionWarnings.ForEach(warningObject => warningObject.SetActive(false));
        _timer = new TimerManager();
        SetNewDelay();
    }

    private void Update()
    {
        if (!_timer.RunTimer(ref _idleTimer, _currentDelay)) return;
        HandleAttackDelay();
        // if (!attackSystem.IsIdle)
        // {
        //     int direction = attackSystem.CurrentAttackDirection();

        //     if (direction >= 0 && direction < attackDirectionWarnings.Count)
        //     {
        //         attackDirectionWarnings[direction].SetActive(true);
        //     }
        // }
        // else
        // {
        //     HandleAttackDelay();
        // }
    }

    private void HandleAttackDelay()
    {
        var attack = GetAttack();
        attackDirectionWarnings[attack.Direction].SetActive(true);
        attackSystem.TriggerAttack(attack);
    }

    private void SetNewDelay()
    {
        _currentDelay = Random.Range(minAttackDelayTime, maxAttackDelayTime);
    }

    private OpponentAttack GetAttack()
    {
        return opponentAttacks[Random.Range(0, opponentAttacks.Count)];
    }

    public void ResetAllAttackWarningDirections()
    {
        attackDirectionWarnings.ForEach(warningObject => warningObject.SetActive(false));
        SetNewDelay();
    }
}