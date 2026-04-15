using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private PlayerDodgeSystem playerDodgeSystem;

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
        Debug.Log("Triggered Attack: " + attack);
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
                Debug.Log("Charging");
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
        if (_currentAttack is OpponentAttack opponentAttack)
        {
            //Debug.Log("Executed Opponent Attack in Direction = " + opponentAttack.Direction);
            //Debug.Log("Damage Done = " + opponentAttack.Damage);
            if (playerDodgeSystem.GetCurrentDodgeInfo().isDodging && playerDodgeSystem.GetCurrentDodgeInfo().dodgeSide == SideType.Left && opponentAttack.Direction == 2)
            {
                Debug.Log("Player Dodged Left attack");
            }
        }
        else
        {
            Debug.Log("Executed Player Punch");
            Debug.Log("Damage Done = " + _currentAttack.Damage);
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
}