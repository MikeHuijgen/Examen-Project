using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSystem : MonoBehaviour
{
    public bool IsIdle => _state == AttackState.Idle;
    public UnityEvent OnEnemyAttackFinished = new UnityEvent();

    public enum AttackState
    {
        Idle,
        Charging,
        Attacking
    }

    [SerializeField] private PlayerDodgeSystem playerDodgeSystem;
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private HealthComponent opponentHealth;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator enemyAnimator;

    private AnimatorOverrideController _playerOverrideController;
    private AnimatorOverrideController _enemyOverrideController;
    private const string _attackTriggerKeyString = "TriggerAttack";
    private const string _windUpAttackTriggerKeyString = "TriggerWindUpAttack";
    private const string _mainAttackTriggerKeyString = "TriggerMainAttack";
    private const string _playerAttackKeyString = "Attack";
    private const string _enemyMainAttackKeyString = "MainAttack";
    private const string _enemyWindUpAttackKeyString = "WindUpAttack";

    private AttackState _state = AttackState.Idle;

    private CountdownTimer _chargeTimer;
    private CountdownTimer _attackTimer;
    private TimerManager _timer;
    private BaseAttack _currentAttack;

    private Queue<BaseAttack> _attackQueue = new Queue<BaseAttack>();
    private bool _hasExecutedAttack;
    private bool _hasExecutedCharge;

    private void Start()
    {
        _timer = new TimerManager();
        _playerOverrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
        _enemyOverrideController = new AnimatorOverrideController(enemyAnimator.runtimeAnimatorController);
        playerAnimator.runtimeAnimatorController = _playerOverrideController;
        enemyAnimator.runtimeAnimatorController = _enemyOverrideController;
    }

    public void QueueAttack(BaseAttack attack)
    {
        Debug.Log($"Queuing attack: {attack.name}");
        _attackQueue.Enqueue(attack);
    }

    private void Update()
    {
        switch (_state)
        {
            case AttackState.Idle:
                CheckQueue();
                break;

            case AttackState.Charging:
                HandleChargeExecution();
                HandleCharging();
                break;

            case AttackState.Attacking:
                HandleAttackExecution();
                HandleAttacking();
                break;
        }
    }

    public void CheckQueue()
    {
        if (_attackQueue.Count > 0)
        {
            TriggerAttack(_attackQueue.Dequeue());
        }
    }

    public void TriggerAttack(BaseAttack attack)
    {
        if (_state != AttackState.Idle)
        {
            _attackQueue.Enqueue(attack);
            return;
        }

        _currentAttack = attack;
        _hasExecutedAttack = false;
        _hasExecutedCharge = false;

        if (_currentAttack is OpponentAttack opponentAttack && opponentAttack.ChargeDurationTime > 0f)
        {
            _state = AttackState.Charging;
        }
        else
        {
            _state = AttackState.Attacking;
        }
    }

    private void HandleChargeExecution()
    {
        if (_hasExecutedCharge)
            return;

        _hasExecutedCharge = true;
        if (_currentAttack is OpponentAttack opponentAttack && opponentAttack.ChargeDurationTime > 0f)
        {
            _enemyOverrideController[_enemyWindUpAttackKeyString] = opponentAttack.ChargeAnimation;
            enemyAnimator.SetTrigger(_windUpAttackTriggerKeyString);
        }
    }

    private void HandleAttackExecution()
    {
        if (_hasExecutedAttack)
            return;

        _hasExecutedAttack = true;

        if (_currentAttack is not OpponentAttack opponentAttack)
        {
            _playerOverrideController[_playerAttackKeyString] = _currentAttack.AttackAnim;
            playerAnimator.SetTrigger(_attackTriggerKeyString);
            return;
        }

        var dodge = playerDodgeSystem.GetCurrentDodgeInfo();
        _enemyOverrideController[_enemyMainAttackKeyString] = opponentAttack.AttackAnim;
        enemyAnimator.SetTrigger(_mainAttackTriggerKeyString);

        if (!dodge.isDodging)
        {
            playerHealth.TakeDamage(_currentAttack.Damage);
            OnEnemyAttackFinished?.Invoke();
            return;
        }

        SideType requiredDodge = GetRequiredDodge(opponentAttack.Direction);

        if (dodge.dodgeSide != requiredDodge)
        {
            playerHealth.TakeDamage(_currentAttack.Damage);
        }

        OnEnemyAttackFinished?.Invoke();
    }

    private void HandleCharging()
    {
        if (_currentAttack is OpponentAttack opponentAttack)
        {
            if (!_timer.RunTimer(ref _chargeTimer, opponentAttack.ChargeDurationTime)) return;
            enemyAnimator.ResetTrigger(_windUpAttackTriggerKeyString);
            _state = AttackState.Attacking;
        }
    }

    private void HandleAttacking()
    {
        if (!_timer.RunTimer(ref _attackTimer, _currentAttack.AttackDurationTime)) return;
        if (_currentAttack is not OpponentAttack opponentAttack)
        {
            opponentHealth.TakeDamage(_currentAttack.Damage);
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