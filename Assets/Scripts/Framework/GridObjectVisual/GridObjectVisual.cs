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
        _gridObject.OnPositionChanged += OnPositionChanged;
        _rectToWorldPosition = rectToWorldPosition;
        OnPositionChanged();
    }

    private void OnPositionChanged()
    {
        transform.DOMove(_rectToWorldPosition(), .15f).SetEase(Ease.InOutQuad);
    }
}
