using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters) : base(parameters){}
    public override async Task Execute()
    {
        var matches = parameters.Matches;
        ActivateMatchEffect(matches);

        parameters.DisposeMatchDataCallback(matches);
        await parameters.DisableMatchesVisualsCallback(matches);
    }

    private void ActivateMatchEffect(HashSet<Match> matches)
    {
        foreach (var match in matches)
        {
            if(match.MatchEffect == null) continue;
            match.MatchEffect.ActivateEffect();
        }
    }
}
