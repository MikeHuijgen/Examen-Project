using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class OpponentBehaviour : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;

    [SerializeField] private List<OpponentAttack> opponentAttacks;
    [SerializeField] private List<GameObject> attackDirectionWarnings;
    [SerializeField] private OnGameOverChanel channel;

    [SerializeField] private float minAttackDelayTime;
    [SerializeField] private float maxAttackDelayTime;

    private CountdownTimer _idleTimer;
    private TimerManager _timer;

    private bool _allowAttack = true;

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
            int direction = attackSystem.CurrentAttackDirection();

            if (direction >= 0 && direction < attackDirectionWarnings.Count)
            {
                attackDirectionWarnings[direction].SetActive(true);
            }
        }
        else
        {
            attackDirectionWarnings.ForEach(warning => warning.SetActive(false));
            HandleAttackDelay();
        }
    }
    private void OnEnable() => channel.OnGameOver += HandleGameOver ;
    private void OnDisable() => channel.OnGameOver -= HandleGameOver;

    private void HandleGameOver()
    {
        _allowAttack = false;
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