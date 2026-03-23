using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class FingerInputTester : MonoBehaviour
{
    public GridSystem gridSystem;
    private void OnEnable() 
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
    }

    private void OnDisable() 
    {
        EnhancedTouchSupport.Disable();  
        Touch.onFingerDown -= OnFingerDown;      
    }

    private void OnFingerDown(Finger finger)
    {
        print(gridSystem.GetWorldToGridPosition(finger.screenPosition));
    }
}
