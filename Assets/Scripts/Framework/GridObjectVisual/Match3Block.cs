using UnityEngine;
using System;

public class Match3Block : MonoBehaviour
{
    [SerializeField] private FakeAttack fakeAttack;
    private Func<Vector3> _rectToWorldPosition;

    public void Initialize(Func<Vector3> rectToWorldPosition)
    {
        _rectToWorldPosition = rectToWorldPosition;
        transform.position = _rectToWorldPosition();
    }

    public Vector3 GetRectPosition() => _rectToWorldPosition();
    public FakeAttack GetFakeAttack => fakeAttack;
}
