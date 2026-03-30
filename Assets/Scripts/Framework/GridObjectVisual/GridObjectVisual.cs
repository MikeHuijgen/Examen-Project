using UnityEngine;
using System;
using DG.Tweening;

public class GridObjectVisual : MonoBehaviour
{
    private GridObject _gridObject;
    private Func<Vector3> _rectToWorldPosition;

    public void Initialize(GridObject gridObject, Func<Vector3> rectToWorldPosition)
    {
        _gridObject = gridObject;
        _rectToWorldPosition = rectToWorldPosition;
        transform.position = _rectToWorldPosition();
    }

    public Vector3 GetRectPosition() => _rectToWorldPosition();
}
