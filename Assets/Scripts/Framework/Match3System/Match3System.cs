using System;
using System.Collections.Generic;

public class Match3System
{
    // public bool CheckForMatchAroundSwappedGridObject(GridPosition[,] grid)
    // {
        
    // }

    public HashSet<Match> GetAllMatches(gridObject[,] grid, Func<gridObject, GridObject> GetGridObjectFromGridPosition)
    {
        foreach (var gridPosition in grid)
        {
            var gridObject = GetGridObjectFromGridPosition(gridPosition);
            var checkValue = gridObject.GetGridMatch3Block.GetFakeAttack;
        }

        return null;
    } 
}
