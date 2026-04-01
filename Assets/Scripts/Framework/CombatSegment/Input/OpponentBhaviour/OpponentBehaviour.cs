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
        if (!attackSystem.IsIdle)
        {
            if(attackSystem.CurrentAttackDirection() == 0)
            {
                attackDirectionWarnings[0].SetActive(true);
            }
            if (attackSystem.CurrentAttackDirection() == 1)
            {
                attackDirectionWarnings[1].SetActive(true);
            }
            if (attackSystem.CurrentAttackDirection() == 2)
            {
                attackDirectionWarnings[2].SetActive(true);
            }
        }
        else
        {
            attackDirectionWarnings.ForEach(warningObject => warningObject.SetActive(false));
            HandleAttackDelay();
        }
    }

    private void HandleAttackDelay()
    {
        if (!_timer.RunTimer(ref _idleTimer, _currentDelay))
            return;

        var attack = GetAttack();
        attackSystem.TriggerAttack(attack);

        SetNewDelay();
    }

    private void SetNewDelay()
    {
        _currentDelay = Random.Range(minAttackDelayTime, maxAttackDelayTime);
    }

    private OpponentAttack GetAttack()
    {
        return opponentAttacks[Random.Range(0, opponentAttacks.Count)];
    }
}