using System;
using System.Collections.Generic;

public struct MatchActionParameters
{
    public HashSet<Match> Matches;
    public Action<HashSet<Match>> DisposeMatchDataCallback;
    public Action<HashSet<Match>> DisableMatchesVisualsCallback;
}
