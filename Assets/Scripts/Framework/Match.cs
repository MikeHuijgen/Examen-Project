using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Match
{
    public GridPosition[] GridPositions;
    public BaseAttack AttackData;

    public Match(BaseAttack attackData, GridPosition[] gridPositions)
    {
        GridPositions = gridPositions;
        AttackData = attackData;
    }
}
