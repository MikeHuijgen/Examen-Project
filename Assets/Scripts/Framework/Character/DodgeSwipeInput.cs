using UnityEngine;

public class DodgeSwipeInput : MonoBehaviour
{
    [SerializeField] private float minimumSwipeDistance = 100f;

    private Vector2 _swipeStartPosition;

    private void OnEnable()
    {
        CharacterInput.Instance.OnNewFingerDownInput += HandleFingerDown;
        CharacterInput.Instance.OnNewFingerUpInput += HandleFingerUp;
    }

    private void OnDisable()
    {
        if (CharacterInput.Instance == null) return;

        CharacterInput.Instance.OnNewFingerDownInput -= HandleFingerDown;
        CharacterInput.Instance.OnNewFingerUpInput -= HandleFingerUp;
    }

    private void HandleFingerDown(Vector2 screenPosition) => _swipeStartPosition = screenPosition;

    private void HandleFingerUp(Vector2 screenPosition)
    {
        Vector2 swipeDelta = screenPosition - _swipeStartPosition;

        if (swipeDelta.magnitude < minimumSwipeDistance)
            return;

        SideType swipeSide = GetSwipeSide(swipeDelta);

        if (swipeSide == SideType.None)
            return;

        CharacterInput.Instance.OnDodgeInputDetected(swipeSide);
    }

    private SideType GetSwipeSide(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            return swipeDelta.x > 0 ? SideType.Right : SideType.Left;

        if (swipeDelta.y < 0)
            return SideType.Down;

        return SideType.None;
    }
}