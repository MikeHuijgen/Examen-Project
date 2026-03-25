using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CharacterInput : MonoBehaviour
{
    public static CharacterInput Instance;

    [SerializeField] private PlayerInput playerInput;
    
    public event Action<SideType> OnDodgeInput;
    
    private Action<InputAction.CallbackContext> _dodgeLeftHandler;
    private Action<InputAction.CallbackContext> _dodgeRightHandler;
    private Action<InputAction.CallbackContext> _dodgeDownHandler;

    public static event Action<Vector2, Vector2> OnNewInputEnded;
    private Vector2 _startPositionGridFinger;
    
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
    }

    private void OnEnable()
    {
        playerInput.actions["DodgeLeft"].performed += _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed += _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed += _dodgeDownHandler;

        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }
    
    private void OnDisable()
    {
        playerInput.actions["DodgeLeft"].performed -= _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed -= _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed -= _dodgeDownHandler;

        EnhancedTouchSupport.Disable();  
        Touch.onFingerDown -= OnFingerDown;      
        Touch.onFingerUp -= OnFingerUp; 
    }

    private void OnDodgeInputDetected(SideType dodgeSide)
    {
        OnDodgeInput?.Invoke(dodgeSide);
    }

    private void OnFingerDown(Finger finger)
    {
        if (_startPositionGridFinger != Vector2.zero) return;
        _startPositionGridFinger = finger.screenPosition;
    }

    private void OnFingerUp(Finger finger)
    {
        var endInputPosition = finger.screenPosition;
        var beginInputPosition = _startPositionGridFinger;
        _startPositionGridFinger = Vector2.zero;
        OnNewInputEnded?.Invoke(beginInputPosition, endInputPosition);
    }
}
