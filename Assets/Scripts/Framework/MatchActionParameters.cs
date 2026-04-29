using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchActionParameters : MonoBehaviour
{
    public HashSet<Match> Matches;
    public Action<HashSet<Match>> DisposeMatchDataCallback;
    public Action<HashSet<Match>> DisableMatchesVisualsCallback;
}
