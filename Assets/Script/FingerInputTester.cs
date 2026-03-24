using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FingerInputTester : MonoBehaviour
{
    public GridSystem gridSystem;
    private Vector2 _startDownFingerPosition;

    private void OnEnable() 
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }


    private void OnDisable() 
    {
        EnhancedTouchSupport.Disable();  
        Touch.onFingerDown -= OnFingerDown;      
        Touch.onFingerUp -= OnFingerUp;      
    }

    private void OnFingerDown(Finger finger)
    {
        if (_startDownFingerPosition != Vector2.zero) return;
        _startDownFingerPosition = finger.screenPosition;
    }

    private void OnFingerUp(Finger finger)
    {
        // hier nog toevoegen dat hij wel echt UI moet aanraken en anders mag hij niet dit doen zelfde als bij down en bij preof proeve project
        // var endDownFingerPosition = finger.screenPosition;
        // var lastDownFinger = _startDownFingerPosition;
        // _startDownFingerPosition = Vector2.zero;

        // var startDownFingerGridPosition = gridSystem.GetWorldToGridPosition(lastDownFinger);
        // var endDownFingerGridPosition = gridSystem.GetWorldToGridPosition(endDownFingerPosition);

        // if (!gridSystem.IsValidGridPosition(startDownFingerGridPosition) || !gridSystem.IsValidGridPosition(endDownFingerGridPosition)) return;
        // print("Test");

    }
}
