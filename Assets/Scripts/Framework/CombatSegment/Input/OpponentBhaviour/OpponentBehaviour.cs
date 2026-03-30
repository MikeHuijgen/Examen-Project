using System.Collections.Generic;
using UnityEngine;

public class OpponentBehaviour : MonoBehaviour
{
    [SerializeField] private List<OpponentAttack> opponentAttacks;
    [SerializeField] private float minAttackDelayTime;
    [SerializeField] private float maxAttackDelayTime;

    private CountdownTimer _idleTimer;
    private CountdownTimer _chargeTimer;
    private CountdownTimer _attackTimer;

    private OpponentState _currentState;
    private OpponentAttack _currentAttack;

    public enum OpponentState
    {
        Idle,
        Charge,
        Attack,
    }

    private void Start()
    {
        TransitionToState(OpponentState.Idle);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case OpponentState.Idle:
                HandleIdle();
                break;

            case OpponentState.Charge:
                HandleCharge();
                break;

            case OpponentState.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandleIdle()
    {
        if (!RunTimer(ref _idleTimer, GetRandomIdleTime()))
            return;
        Debug.Log("Charging");
        TransitionToState(OpponentState.Charge);
    }

    private void HandleCharge()
    {
        if (!RunTimer(ref _chargeTimer, _currentAttack.ChargeDurationTime))
            return;
        Debug.Log("Attacking");
        TransitionToState(OpponentState.Attack);
    }

    private void HandleAttack()
    {
        if (!RunTimer(ref _attackTimer, _currentAttack.AttackDurationTime))
            return;

        Debug.Log("Idle");
        TransitionToState(OpponentState.Idle);
    }

    private void TransitionToState(OpponentState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case OpponentState.Idle:
                break;

            case OpponentState.Charge:
                _currentAttack = GetAttack();
                break;

            case OpponentState.Attack:
                break;
        }
    }

    private bool RunTimer(ref CountdownTimer timer, float duration)
    {
        if (timer == null || !timer.IsTimerActive)
        {
            timer = new CountdownTimer(duration);
            timer.StartTimer();
            return false;
        }

        timer.Tick(Time.deltaTime);

        if (!timer.IsTimerDone)
            return false;

        timer.StopTimer();
        return true;
    }

    private OpponentAttack GetAttack()
    {
        return opponentAttacks[Random.Range(0, opponentAttacks.Count)];
    }

    private float GetRandomIdleTime()
    {
        return Random.Range(minAttackDelayTime, maxAttackDelayTime);
    }
}