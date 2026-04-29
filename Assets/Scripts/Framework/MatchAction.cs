using System.Threading.Tasks;
using UnityEngine;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters) : base(parameters){}
    public override async Task Execute()
    {
        var matches = parameters.Matches;
        parameters.DisposeMatchDataCallback(matches);
        await parameters.DisableMatchesVisualsCallback(matches);
    }
}
