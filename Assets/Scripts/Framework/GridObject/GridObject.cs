using System;
using UnityEngine;

public class GridObject
{
    public event Action OnPositionChanged;
    private GridPosition _gridPosition;
    private GridObjectVisualUI _gridObjectVisualUI;
    private GridObjectVisual _gridObjectVisual;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public GridObjectVisualUI GetGridObjectVisualUI => _gridObjectVisualUI;
    public GridObjectVisual GetGridObjectVisual => _gridObjectVisual;
    public void SetGridObjectVisualUI(GridObjectVisualUI gridObjectVisualUI) => _gridObjectVisualUI = gridObjectVisualUI;
    public void SetGridObjectVisual(GridObjectVisual gridObjectVisual) => _gridObjectVisual = gridObjectVisual;


    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition; 
        OnPositionChanged?.Invoke();
    }
}
