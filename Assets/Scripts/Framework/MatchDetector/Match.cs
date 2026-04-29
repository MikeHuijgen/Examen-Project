using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Match
{
    public GridObject[] MatchedObjectGroup;
    public BaseMatchEffect MatchEffect;

    public Match(GridObject[] matchedObjectGroup, BaseMatchEffect matchEffect)
    {
        MatchedObjectGroup = matchedObjectGroup;
        MatchEffect = matchEffect;
    }
}
