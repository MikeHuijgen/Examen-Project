using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Match
{
    public GridObject[] MatchedObjectGroup;

    public Match(GridObject[] matchedObjectGroup)
    {
        MatchedObjectGroup = matchedObjectGroup;
    }
}
