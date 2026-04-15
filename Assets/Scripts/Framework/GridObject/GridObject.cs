using System;
using UnityEngine;

public class GridObject
{
    private GridPosition _gridPosition;
    private FakeAttack _fakeAttackData;
    private Match3Block _match3Block;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public Vector3 GetWorldPosition(float cellWidth, float cellHeight) => new Vector3(_gridPosition.X * cellWidth + cellWidth / 2, _gridPosition.Y * cellHeight + cellHeight / 2, 0);
    public Match3Block GetGridMatch3Block => _match3Block;
    public FakeAttack GetAttackData => _fakeAttackData;
    public void SetMatch3Block(Match3Block match3Block) => _match3Block = match3Block;
    public void SetAttackData(FakeAttack attackData) => _fakeAttackData = attackData;
    public void SetGridPosition(GridPosition gridPosition) => _gridPosition = gridPosition; 
}
