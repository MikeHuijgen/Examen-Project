using System.Collections.Generic;

public struct Match
{
    public List<GridPosition> matchGridPosition;

    public void Init() => matchGridPosition = new List<GridPosition>();

    public void AddToMatch(GridPosition gridPosition)
    {
        if (matchGridPosition.Contains(gridPosition)) return;
        matchGridPosition.Add(gridPosition);
    }
}
