using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Match
{
    public GridObject[] MatchedObjectGroup;
    public BaseAttack MatchAttack;

    public Match(GridObject[] matchedObjectGroup, BaseAttack attack)
    {
        MatchedObjectGroup = matchedObjectGroup;
        MatchAttack = attack;
    }
}
