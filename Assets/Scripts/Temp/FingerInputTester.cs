using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FingerInputTester : MonoBehaviour
{   
    public static event Action<Vector2, Vector2> OnNewInputEnded;
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
        var endInputPosition = finger.screenPosition;
        var beginInputPosition = _startDownFingerPosition;
        _startDownFingerPosition = Vector2.zero;
        OnNewInputEnded?.Invoke(beginInputPosition, endInputPosition);
    }
}
