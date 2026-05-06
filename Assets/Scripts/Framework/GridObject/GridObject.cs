using UnityEngine;

public class GridObject
{
    private GridPosition _gridPosition;
    private Match3BlockProfile _match3BlockProfile;

    public GridObject(GridPosition gridPosition) => _gridPosition = gridPosition;
    public GridPosition GetGridPosition => _gridPosition;
    public Vector3 GetWorldPosition(float cellWidth, float cellHeight) => new Vector3(_gridPosition.X * cellWidth + cellWidth / 2, _gridPosition.Y * cellHeight + cellHeight / 2, 0);
    public Match3BlockProfile GetMatch3BlockProfile => _match3BlockProfile;
    public void SetGridPosition(GridPosition gridPosition) => _gridPosition = gridPosition; 
    public void SetMatch3BlockProfile(Match3BlockProfile match3BlockProfile) => _match3BlockProfile = match3BlockProfile;
}
