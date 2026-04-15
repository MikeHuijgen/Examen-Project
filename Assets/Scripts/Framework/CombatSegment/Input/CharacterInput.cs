using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CharacterInput : MonoBehaviour
{
    public static CharacterInput Instance;
    public event Action<Vector2> OnNewFingerDownInput;
    public event Action<Vector2> OnNewFingerUpInput;
    public event Action<SideType> OnDodgeInput;

    //temp Action
    public event Action<BaseAttack> OnPlayerAttack;

    //private Func<Vector2, bool, GridPosition?, GridPosition?> _isValidGridPositionCallback;

    private Action<InputAction.CallbackContext> _dodgeLeftHandler;
    private Action<InputAction.CallbackContext> _dodgeRightHandler;
    private Action<InputAction.CallbackContext> _dodgeDownHandler;

    //Temp Inputs
    private Action<InputAction.CallbackContext> _firstAttackHandler;
    private Action<InputAction.CallbackContext> _secondAttackHandler;
    private Action<InputAction.CallbackContext> _thirdAttackHandler;
    private Action<InputAction.CallbackContext> _fourthAttackHandler;
    private Action<InputAction.CallbackContext> _fifthAttackHandler;

    [SerializeField] private PlayerAttackTest _playerAttackTest;
    [SerializeField] private PlayerInput playerInput;
    
    private void Awake()
    {
        Debug.Log(_playerAttackTest);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        _dodgeLeftHandler = ctx => OnDodgeInputDetected(SideType.Left);
        _dodgeRightHandler = ctx => OnDodgeInputDetected(SideType.Right);
        _dodgeDownHandler = ctx => OnDodgeInputDetected(SideType.Down);


        //temp Attack handlers
        _firstAttackHandler = ctx => _playerAttackTest.TriggerAttack(1);
        _secondAttackHandler = ctx => _playerAttackTest.TriggerAttack(2);
        _thirdAttackHandler = ctx => _playerAttackTest.TriggerAttack(3);
        _fourthAttackHandler = ctx => _playerAttackTest.TriggerAttack(4);
        _fifthAttackHandler = ctx => _playerAttackTest.TriggerAttack(5);

    }

    private void OnEnable()
    {
        playerInput.actions["DodgeLeft"].performed += _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed += _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed += _dodgeDownHandler;

        //temp Inputs
        playerInput.actions["firstAttack"].performed += _firstAttackHandler;
        playerInput.actions["secondAttack"].performed += _secondAttackHandler;
        playerInput.actions["thirdAttack"].performed += _thirdAttackHandler;
        playerInput.actions["fourthAttack"].performed += _fourthAttackHandler;
        playerInput.actions["fifthAttack"].performed += _fifthAttackHandler;

        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        playerInput.actions["DodgeLeft"].performed -= _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed -= _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed -= _dodgeDownHandler;

        //temp Inputs
        playerInput.actions["firstAttack"].performed -= _firstAttackHandler;
        playerInput.actions["secondAttack"].performed -= _secondAttackHandler;
        playerInput.actions["thirdAttack"].performed -= _thirdAttackHandler;
        playerInput.actions["fourthAttack"].performed -= _fourthAttackHandler;
        playerInput.actions["fifthAttack"].performed -= _fifthAttackHandler;

        EnhancedTouchSupport.Disable();
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;
    }

    public void OnDodgeInputDetected(SideType dodgeSide) => OnDodgeInput?.Invoke(dodgeSide);

    private void OnFingerDown(Finger finger) => OnNewFingerDownInput?.Invoke(finger.screenPosition);

    private void OnFingerUp(Finger finger) => OnNewFingerUpInput?.Invoke(finger.screenPosition);
}
