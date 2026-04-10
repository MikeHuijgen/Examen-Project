using System.Collections.Generic;

public struct Match
{
    public List<gridObject> matchGridPosition;

    public void Init() => matchGridPosition = new List<gridObject>();

    public void AddToMatch(gridObject gridPosition)
    {
        if (matchGridPosition.Contains(gridPosition)) return;
        matchGridPosition.Add(gridPosition);
    }
}
