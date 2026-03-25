using System;

public class GridObject
{
    private GridPosition _gridPosition;
    private GridObjectDebugVisual _gridObjectDebugVisual;
    private Action<GridPosition> _onPositionChanged;

    public GridObject(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public GridPosition GetGridPosition => _gridPosition;
    public GridObjectDebugVisual GridObjectDebug => _gridObjectDebugVisual;
    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition; 
        _onPositionChanged?.Invoke(_gridPosition);
    }

    public void SetVisual(GridObjectDebugVisual gridObjectDebugVisual)
    {
        _gridObjectDebugVisual = gridObjectDebugVisual;
    }

    public void PositionChangedCallback(Action<GridPosition> callBack)
    {
        _onPositionChanged = callBack;
    }
}
