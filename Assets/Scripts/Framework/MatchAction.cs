using System.Threading.Tasks;
using UnityEngine;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters) : base(parameters){}
    public override async Task Execute()
    {
        var matches = parameters.Matches;
        parameters.DisableMatchesVisualsCallback(matches);
        parameters.DisposeMatchDataCallback(matches);
    }
}
