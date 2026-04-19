using System;
using UnityEngine;

public class GridObject
{
    private GridPosition _gridPosition;
    private BaseAttack _attackData;
    private Match3BlockVisual _match3Block;
    private Match3BlockProfile _match3BlockProfile;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public Vector3 GetWorldPosition(float cellWidth, float cellHeight) => new Vector3(_gridPosition.X * cellWidth + cellWidth / 2, _gridPosition.Y * cellHeight + cellHeight / 2, 0);
    public Match3BlockVisual GetGridMatch3Block => _match3Block;
    public BaseAttack GetAttackData => _attackData;
    public Match3BlockProfile GetMatch3BlockProfile => _match3BlockProfile;
    public void SetMatch3Block(Match3BlockVisual match3Block) => _match3Block = match3Block;
    public void SetAttackData(BaseAttack attackData) => _attackData = attackData;
    public void SetGridPosition(GridPosition gridPosition) => _gridPosition = gridPosition; 
    public void SetMatch3BlockProfile(Match3BlockProfile match3BlockProfile) => _match3BlockProfile = match3BlockProfile;
}
