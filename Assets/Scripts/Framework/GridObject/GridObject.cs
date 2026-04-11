using System;
using UnityEngine;

public class GridObject
{
    public event Action OnPositionChanged;
    private GridPosition _gridPosition;
    private GridObjectVisualUI _gridObjectVisualUI;
    private FakeAttack _fakeAttackData;
    private Match3Block _match3Block;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public GridObjectVisualUI GetGridObjectVisualUI => _gridObjectVisualUI;
    public Match3Block GetGridMatch3Block => _match3Block;
    public FakeAttack GetAttackData => _fakeAttackData;
    public void SetGridObjectVisualUI(GridObjectVisualUI gridObjectVisualUI) => _gridObjectVisualUI = gridObjectVisualUI;
    public void SetMatch3Block(Match3Block match3Block) => _match3Block = match3Block;
    public void SetAttackData(FakeAttack attackData) => _fakeAttackData = attackData;

    public void SetGridPosition(GridPosition gridPosition)
    {
        _gridPosition = gridPosition; 
        OnPositionChanged?.Invoke();
    }
}
