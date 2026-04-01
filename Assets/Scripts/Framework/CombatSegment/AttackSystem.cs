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
                break;

            case AttackState.Charging:
                Debug.Log("Charging");
                HandleCharging();
                break;

            case AttackState.Attacking:
                if(_currentAttack.ChargeDurationTime > 0f)
                {
                    Debug.Log("Executed Opponenet Attack in This Direction = " + CurrentAttackDirection());
                    Debug.Log("Damage Done = " + _currentAttack.Damage);
                }
                else
                {
                    Debug.Log("Executed Player Punch");
                    Debug.Log("Damage Done = " + _currentAttack.Damage);
                }
                HandleAttacking();
                break;
        }
    }

    private void HandleCharging()
    {
        if (!_timer.RunTimer(ref _chargeTimer, _currentAttack.ChargeDurationTime))
        {
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
