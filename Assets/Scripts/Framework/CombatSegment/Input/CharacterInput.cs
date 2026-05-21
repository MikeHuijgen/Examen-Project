using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CharacterInput : MonoBehaviour
{
    public static CharacterInput Instance { get; private set; }

    public event Action<Vector2> OnNewFingerDownInput;
    public event Action<Vector2> OnNewFingerUpInput;
    public event Action<SideType> OnDodgeInput;

    private bool _allowInput = true;

    [SerializeField] private OnGameOverChannel channel;
    [SerializeField] private PlayerInput _playerInput;

    private readonly Dictionary<string, SideType> _dodgeBindings = new()
    {
        { "DodgeLeft", SideType.Left },
        { "DodgeRight", SideType.Right },
        { "DodgeDown", SideType.Down }
    };

    private readonly Dictionary<string, Action<InputAction.CallbackContext>> _dodgeHandlers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var pair in _dodgeBindings)
        {
            var side = pair.Value;
            _dodgeHandlers[pair.Key] = ctx => OnDodgeInputDetected(side);
        }
    }

    private void OnEnable()
    {
        foreach (var pair in _dodgeHandlers) Bind(pair.Key, pair.Value);

        channel.OnGameOver += HandleGameOver;
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        foreach (var pair in _dodgeHandlers) Unbind(pair.Key, pair.Value);

        channel.OnGameOver -= HandleGameOver;
        EnhancedTouchSupport.Disable();
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;
    }
    private void HandleGameOver()
    {
        _allowInput = false;
    }
    private void Bind(string actionName, Action<InputAction.CallbackContext> handler) => _playerInput.actions[actionName].performed += handler;

    private void Unbind(string actionName, Action<InputAction.CallbackContext> handler) => _playerInput.actions[actionName].performed -= handler;

    public void OnDodgeInputDetected(SideType dodgeSide)
    { 
        if (!_allowInput) return;
        OnDodgeInput?.Invoke(dodgeSide); 
    }

    private void OnFingerDown(Finger finger)
    {
        if (!_allowInput) return;
        OnNewFingerDownInput?.Invoke(finger.screenPosition);
    }

    private void OnFingerUp(Finger finger)
    {
        if (!_allowInput) return;
        OnNewFingerUpInput?.Invoke(finger.screenPosition);
    }
}
