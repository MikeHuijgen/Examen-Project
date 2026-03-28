using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CharacterInput : MonoBehaviour
{
    public static CharacterInput Instance;
    public event Action<SideType> OnDodgeInput;
    public event Action<GridPosition?> OnGridPositionSelected;
    public event Action<GridPosition?> OnGridPositionDeselected;
    private Action<GridPosition, GridPosition> _onRequestGridObjectSwap;
    private Func<Vector2, bool, GridPosition?, GridPosition?> _isValidGridPositionCallback;
    private Action<InputAction.CallbackContext> _dodgeLeftHandler;
    private Action<InputAction.CallbackContext> _dodgeRightHandler;
    private Action<InputAction.CallbackContext> _dodgeDownHandler;

    [SerializeField] private PlayerInput playerInput;

    private GridPosition? _lastGridPositionCache;
    private GridPosition? _beginTouchGridPosition;
    private GridPosition? _endTouchGridPosition;
    
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
        if (_isValidGridPositionCallback == null) return;
        var gridPosition = _isValidGridPositionCallback(finger.screenPosition, false, null);
        if(gridPosition == null) 
        {
            _beginTouchGridPosition = null;
            return;
        }
        
        _beginTouchGridPosition = gridPosition;

        if(_lastGridPositionCache != null) return;
        _lastGridPositionCache = gridPosition;
        OnGridPositionSelected?.Invoke(_lastGridPositionCache.Value);
    }

    private void OnFingerUp(Finger finger)
    {
        if (_isValidGridPositionCallback == null) return;
        
        if (_beginTouchGridPosition == _lastGridPositionCache)
            _endTouchGridPosition = _isValidGridPositionCallback(finger.screenPosition, false, _lastGridPositionCache);
        else
            _endTouchGridPosition = _isValidGridPositionCallback(finger.screenPosition, true, _lastGridPositionCache);            
        
        if(_beginTouchGridPosition == _endTouchGridPosition) return;

        if (_lastGridPositionCache == null) return;

        if (_endTouchGridPosition == null)
        {
            OnGridPositionDeselected?.Invoke(_lastGridPositionCache);   
            _lastGridPositionCache = null;   
            return;      
        }

        OnGridPositionDeselected?.Invoke(_lastGridPositionCache);

        _onRequestGridObjectSwap(_lastGridPositionCache.Value, _endTouchGridPosition.Value);

        _lastGridPositionCache = null;
    }

    public void SetOnRequestGridObjectSwapCallback(Action<GridPosition, GridPosition> callback) => _onRequestGridObjectSwap = callback;
    public void SetIsValidGridPositionCallback(Func<Vector2, bool, GridPosition?, GridPosition?> callback) => _isValidGridPositionCallback = callback;
}
