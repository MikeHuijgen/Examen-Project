using UnityEngine;
using System;

public class GridObjectVisual : MonoBehaviour
{
    private GridObject _gridObject;
    private Func<Vector3> _rectToWorldPosition;

    public void Initialize(GridObject gridObject, Func<Vector3> rectToWorldPosition)
    {
        _gridObject = gridObject;
        _gridObject.OnPositionChanged += OnPositionChanged;
        _rectToWorldPosition = rectToWorldPosition;
        OnPositionChanged();
    }

    private void OnPositionChanged()
    {
        transform.position = _rectToWorldPosition();
    }
}
