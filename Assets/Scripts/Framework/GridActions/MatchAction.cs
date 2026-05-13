using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters, ActionContext context) : base(parameters, context){}

    public override void Execute(Action<BaseAction> onActionComplete)
    {
        on_action_complete = onActionComplete;
        var matches = parameters.Matches;
        ActivateMatchEffect(matches);

        action_context.GridSystem.DisposeMatchData(matches);
        action_context.BlockVisualManager.DisableMatchesVisuals(matches);

        action_context.GridActionProcessor.ProcessAction
        (
            new CollapseAndFillAction
            (
                new CollapseAndFillActionParameters
                {
                    Match3BlockProfiles = action_context.Match3BlockProfileContainer.match3BlockProfiles
                },
                action_context
            ),
            _ =>
            {
                CompleteAction();
            }
        );
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
