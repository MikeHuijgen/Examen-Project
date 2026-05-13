using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters, ActionContext context) : base(parameters, context){}

    public override void Execute(Action onActionComplete)
    {
        on_action_complete = onActionComplete;
        var matches = parameters.Matches;
        ActivateMatchEffect(matches);

        action_context.GridSystem.DisposeMatchData(matches);
        action_context.BlockVisualManager.DisableMatchesVisuals(matches);
        Debug.Log("Match action done");
        on_action_complete();
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
