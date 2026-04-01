using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    private AttackState _state = AttackState.Idle;

    private CountdownTimer _chargeTimer;
    private CountdownTimer _attackTimer;
    private TimerManager _timer;
    private OpponentAttack _currentAttack;

    public bool IsIdle => _state == AttackState.Idle;

    public enum AttackState
    {
        Idle,
        Charging,
        Attacking
    }

    private void Start()
    {
        _timer = new TimerManager();
    }


    public void TriggerAttack(OpponentAttack attack)
    {
        if (!IsIdle) return;

        _currentAttack = attack;

        if (_currentAttack.ChargeDurationTime > 0f)
        {
            _state = AttackState.Charging;
        }
        else
        {
            _state = AttackState.Attacking;
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case AttackState.Idle:
                Debug.Log("Idle");
                break;

            case AttackState.Charging:
                HandleCharging();
                break;

            case AttackState.Attacking:
                Debug.Log("Attacking in This Direction = " + CurrentAttackDirection());
                HandleAttacking();
                break;
        }
    }

    private void HandleCharging()
    {
        if (!_timer.RunTimer(ref _chargeTimer, _currentAttack.ChargeDurationTime))
        {
            Debug.Log("Charging");
            return;
        }

        _state = AttackState.Attacking;
    }

    private void HandleAttacking()
    {
        if (!_timer.RunTimer(ref _attackTimer, _currentAttack.AttackDurationTime))
        {
            return;
        }

        _state = AttackState.Idle;
    }

    public int CurrentAttackDirection()
    {
        if (IsIdle) return -1;
        return _currentAttack.Direction;
    }
}
