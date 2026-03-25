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
    private Vector2 _startGridFingerPosition;
    private Vector2 _lastGridFingerPosition;
    private bool _canReadFingerUpInput = true;
    
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
        GridSystem.OnSwappedGridObjects += OnSwappedGridObjects;

        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        playerInput.actions["DodgeLeft"].performed -= _dodgeLeftHandler;
        playerInput.actions["DodgeRight"].performed -= _dodgeRightHandler;
        playerInput.actions["DodgeDown"].performed -= _dodgeDownHandler;
        GridSystem.OnSwappedGridObjects += OnSwappedGridObjects;

        EnhancedTouchSupport.Disable();  
        Touch.onFingerDown -= OnFingerDown;      
        Touch.onFingerUp -= OnFingerUp; 
    }

    private void OnSwappedGridObjects() => _canReadFingerUpInput = true;

    private void OnDodgeInputDetected(SideType dodgeSide)
    {
        OnDodgeInput?.Invoke(dodgeSide);
    }

    private void OnFingerDown(Finger finger)
    {
        if (_lastGridFingerPosition != Vector2.zero && _lastGridFingerPosition != finger.screenPosition)
        {
            OnNewInputEnded?.Invoke(_lastGridFingerPosition, finger.screenPosition);
            _canReadFingerUpInput = false;
            _lastGridFingerPosition = Vector2.zero;            
            return;
        }

        _startGridFingerPosition = finger.screenPosition;
    }

    private void OnFingerUp(Finger finger)
    {
        var endInputPosition = finger.screenPosition;
        var beginInputPosition = _startGridFingerPosition;

        if (beginInputPosition == endInputPosition) 
        {
            _lastGridFingerPosition = beginInputPosition;
            _startGridFingerPosition = Vector2.zero;
            return;
        }

        if (!_canReadFingerUpInput) return;

        _startGridFingerPosition = Vector2.zero;
        OnNewInputEnded?.Invoke(beginInputPosition, endInputPosition);
        _canReadFingerUpInput = false;
    }
}
