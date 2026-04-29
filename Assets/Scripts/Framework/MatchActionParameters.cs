using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public struct MatchActionParameters
{
    public HashSet<Match> Matches;
    public Action<HashSet<Match>> DisposeMatchDataCallback;
    public Func<HashSet<Match>, Task> DisableMatchesVisualsCallback;
}
