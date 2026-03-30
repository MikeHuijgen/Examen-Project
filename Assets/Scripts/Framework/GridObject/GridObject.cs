using System;
using UnityEngine;

public class GridObject
{
    public event Action OnPositionChanged;
    private GridPosition _gridPosition;
    private GridObjectVisualUI _gridObjectVisualUI;
    private Match3Block _gridObjectVisual;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public GridObjectVisualUI GetGridObjectVisualUI => _gridObjectVisualUI;
    public Match3Block GetGridObjectVisual => _gridObjectVisual;
    public void SetGridObjectVisualUI(GridObjectVisualUI gridObjectVisualUI) => _gridObjectVisualUI = gridObjectVisualUI;
    public void SetGridObjectVisual(Match3Block gridObjectVisual) => _gridObjectVisual = gridObjectVisual;


    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition; 
        OnPositionChanged?.Invoke();
    }
}
