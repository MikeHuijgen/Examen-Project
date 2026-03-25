using System;

public class GridObject
{
    private GridPosition _gridPosition;
    private GridObjectVisualUI _gridObjectVisualUI;
    private Action<GridPosition> _onPositionChanged;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public GridObjectVisualUI GetGridObjectVisualUI => _gridObjectVisualUI;
    public void SetGridObjectVisualUI(GridObjectVisualUI gridObjectDebugVisual) => _gridObjectVisualUI = gridObjectDebugVisual;
    public void PositionChangedCallback(Action<GridPosition> callBack) => _onPositionChanged = callBack;

    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition; 
        _onPositionChanged?.Invoke(_gridPosition);
    }
}
