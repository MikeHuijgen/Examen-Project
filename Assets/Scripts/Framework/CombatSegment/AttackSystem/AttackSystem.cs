using System;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private PlayerDodgeSystem playerDodgeSystem;
    [SerializeField] private HealthComponent _playerHealth;
    [SerializeField] private HealthComponent _opponentHealth;

    private AttackState _state = AttackState.Idle;

    private CountdownTimer _chargeTimer;
    private CountdownTimer _attackTimer;
    private TimerManager _timer;

    private BaseAttack _currentAttack;

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

    public void TriggerAttack(BaseAttack attack)
    {
        if (!IsIdle) return;

        _currentAttack = attack;

        if (_currentAttack is OpponentAttack opponentAttack && opponentAttack.ChargeDurationTime > 0f)
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
                HandleCharging();
                break;

            case AttackState.Attacking:
                HandleAttackExecution();
                HandleAttacking();
                break;
        }
    }

    private void HandleAttackExecution()
    {
        if (_currentAttack is not OpponentAttack opponentAttack)
        {
            _opponentHealth.TakeDamage(_currentAttack.Damage);
            _state = AttackState.Idle;
            return;
        }

        var dodge = playerDodgeSystem.GetCurrentDodgeInfo();

        if (!dodge.isDodging)
        {
            _playerHealth.TakeDamage(_currentAttack.Damage);
            _state = AttackState.Idle;
        }

        SideType requiredDodge = GetRequiredDodge(opponentAttack.Direction);

        if (dodge.dodgeSide == requiredDodge)
        {
            Debug.Log($"Player Dodged {requiredDodge} attack");
        }
        else
        {
            _playerHealth.TakeDamage(_currentAttack.Damage);
            _state = AttackState.Idle;
        }
    }

    private void HandleCharging()
    {
        if (_currentAttack is OpponentAttack opponentAttack && opponentAttack.ChargeDurationTime > 0f)
        {
            if (!_timer.RunTimer(ref _chargeTimer, opponentAttack.ChargeDurationTime))
            {
                return;
            }
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

        if (_currentAttack is OpponentAttack opponentAttack)
        {
            return opponentAttack.Direction;
        }

        return -1;
    }

    private SideType GetRequiredDodge(int direction)
    {
        return direction switch
        {
            0 => SideType.Right,
            1 => SideType.Down,
            2 => SideType.Left,
            _ => throw new Exception("Invalid attack direction")
        };
    }
}