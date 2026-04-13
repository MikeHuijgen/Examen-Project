using System;
using Unity.VisualScripting;
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


    //temp
    public event Action<BaseAttack> OnPlayerAttack;

    private Func<Vector2, bool, GridPosition?, GridPosition?> _isValidGridPositionCallback;
    private Action<InputAction.CallbackContext> _dodgeLeftHandler;
    private Action<InputAction.CallbackContext> _dodgeRightHandler;
    private Action<InputAction.CallbackContext> _dodgeDownHandler;

    //Temp Inputs
    private Action<InputAction.CallbackContext> _firstAttackHandler;
    private Action<InputAction.CallbackContext> _secondAttackHandler;
    private Action<InputAction.CallbackContext> _thirdAttackHandler;
    private Action<InputAction.CallbackContext> _fourthAttackHandler;

    [SerializeField] private PlayerInput playerInput;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        _dodgeLeftHandler = ctx => OnDodgeInputDetected(SideType.Left);
        _dodgeRightHandler = ctx => OnDodgeInputDetected(SideType.Right);
        _dodgeDownHandler = ctx => OnDodgeInputDetected(SideType.Down);


        //temp Attacks
        _firstAttackHandler = ctx => Debug.Log("first Attack");
        _secondAttackHandler = ctx => Debug.Log("second Attack");
        _thirdAttackHandler = ctx => Debug.Log("third Attack");
        _fourthAttackHandler = ctx => Debug.Log("fourth Attack");
    }

    private void OnEnable()
    {
        playerInput.actions["DodgeLeft"].performed += _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed += _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed += _dodgeDownHandler;

        //temp Attacks
        playerInput.actions["firstAttack"].performed += _firstAttackHandler;
        playerInput.actions["secondAttack"].performed += _secondAttackHandler;
        playerInput.actions["thirdAttack"].performed += _thirdAttackHandler;
        playerInput.actions["fourthAttack"].performed += _fourthAttackHandler;

        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        playerInput.actions["DodgeLeft"].performed -= _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed -= _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed -= _dodgeDownHandler;

        //temp Attacks
        playerInput.actions["firstAttack"].performed -= _firstAttackHandler;
        playerInput.actions["secondAttack"].performed -= _secondAttackHandler;
        playerInput.actions["thirdAttack"].performed -= _thirdAttackHandler;
        playerInput.actions["fourthAttack"].performed -= _fourthAttackHandler;

        EnhancedTouchSupport.Disable();  
        Touch.onFingerDown -= OnFingerDown;      
        Touch.onFingerUp -= OnFingerUp; 
    }
   
    //temp
    //public string OnPlayerAttackDetected();

    public void OnDodgeInputDetected(SideType dodgeSide) => OnDodgeInput?.Invoke(dodgeSide);

    private void OnFingerDown(Finger finger) => OnNewFingerDownInput?.Invoke(finger.screenPosition);

    private void OnFingerUp(Finger finger) => OnNewFingerUpInput?.Invoke(finger.screenPosition);
}
