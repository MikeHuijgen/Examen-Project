using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSystem : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private MatchDoubleDamageEffectChannel matchDoubleDamageEffectChannel;

    [Header("System Class References")]
    [SerializeField] private PlayerDodgeSystem playerDodgeSystem;
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private HealthComponent opponentHealth;

    [Header("Animators")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator enemyAnimator;

    public enum AttackState
    {
        Idle,
        Charging,
        Attacking
    }
    [Header("Events")]
    public UnityEvent OnEnemyAttackFinished = new UnityEvent();

    private CountdownTimer _chargeTimer;
    private CountdownTimer _attackTimer;
    private TimerManager _timer;
    private BaseAttack _currentAttack;

    private AttackState _state = AttackState.Idle;

    private AnimatorOverrideController _playerOverrideController;
    private AnimatorOverrideController _enemyOverrideController;

    private Queue<BaseAttack> _attackQueue = new Queue<BaseAttack>();

    private const string _attackTriggerKeyString = "TriggerAttack";
    private const string _windUpAttackTriggerKeyString = "TriggerWindUpAttack";
    private const string _mainAttackTriggerKeyString = "TriggerMainAttack";
    private const string _playerAttackKeyString = "Attack";
    private const string _enemyMainAttackKeyString = "MainAttack";
    private const string _enemyWindUpAttackKeyString = "WindUpAttack";

    private bool IsIdle => _state == AttackState.Idle;
    private bool _hasExecutedAttack;
    private bool _hasExecutedCharge;

    private float _multiplier = 1f;

    private void Start()
    {
        _timer = new TimerManager();
        _playerOverrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
        _enemyOverrideController = new AnimatorOverrideController(enemyAnimator.runtimeAnimatorController);
        playerAnimator.runtimeAnimatorController = _playerOverrideController;
        enemyAnimator.runtimeAnimatorController = _enemyOverrideController;
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
                HandleChargeOutput();
                break;

            case AttackState.Attacking:
                HandleAttackExecution();
                HandleAttackOutput();
                break;
        }
    }

    private void OnEnable() => matchDoubleDamageEffectChannel.OnEventRaised += HandleDoubleDamageEffect;
    private void OnDisable() => matchDoubleDamageEffectChannel.OnEventRaised -= HandleDoubleDamageEffect;

    public void QueueAttack(BaseAttack attack)
    {
        _attackQueue.Enqueue(attack);
    }

    public void CheckQueue()
    {
        if (_attackQueue.Count > 0)
        {
            TriggerAttack(_attackQueue.Dequeue());
        }
    }

    private void TriggerAttack(BaseAttack attack)
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

    private void HandleChargeOutput()
    {
        if (_currentAttack is OpponentAttack opponentAttack)
        {
            if (!_timer.RunTimer(ref _chargeTimer, opponentAttack.ChargeDurationTime)) return;
            enemyAnimator.ResetTrigger(_windUpAttackTriggerKeyString);
            _state = AttackState.Attacking;
        }
    }

    private void HandleAttackOutput()
    {
        if (!_timer.RunTimer(ref _attackTimer, _currentAttack.AttackDurationTime)) return;
        if (_currentAttack is not OpponentAttack opponentAttack)
        {
            var totalDamage = _currentAttack.Damage * _multiplier;
            opponentHealth.TakeDamage(totalDamage);
            _multiplier = 1f;
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

    private void HandleDoubleDamageEffect(float damageMultiplier)
    {
        _multiplier = damageMultiplier;
    }
}