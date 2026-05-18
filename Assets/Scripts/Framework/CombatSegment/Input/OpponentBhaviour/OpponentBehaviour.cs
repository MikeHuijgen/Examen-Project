using System.Collections.Generic;
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

    private bool _allowAttack;

    private float _currentDelay;

    private void Start()
    {
        attackDirectionWarnings.ForEach(warningObject => warningObject.SetActive(false));
        _timer = new TimerManager();
        SetNewDelay();
    }

    private void Update()
    {
        if (!_timer.RunTimer(ref _idleTimer, _currentDelay) || !_allowAttack) return;
        HandleAttackDelay();
    }
    private void OnEnable() => channel.OnGameOver += HandleGameOver ;
    private void OnDisable() => channel.OnGameOver -= HandleGameOver;

    private void HandleGameOver() => SetAllowAttack(false);

    private void HandleAttackDelay()
    {
        var attack = GetAttack();
        attackDirectionWarnings[attack.Direction].SetActive(true);
        attackSystem.QueueAttack(attack);
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

    public void ResetTimer()
    {
        _idleTimer.ResetTimer();
        SetNewDelay();
    }

    public void SetAllowAttack(bool value) => _allowAttack = value;
}