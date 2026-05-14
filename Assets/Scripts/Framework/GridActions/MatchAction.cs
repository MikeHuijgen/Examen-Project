using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchAction : BaseAction<MatchActionParameters>
{
    public MatchAction(MatchActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> onActionComplete)
    {
        on_action_complete = onActionComplete;
        action_context = parameters.actionContext;
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
                    actionContext = action_context,
                }
            ), _ => { CompleteAction(); }
        );
    }

    private void ActivateMatchEffect(HashSet<Match> matches)
    {
        foreach (var match in matches)
        {
            if (IsCanceled) break;
            if (match.MatchEffect == null) continue;
            match.MatchEffect.ActivateEffect();
        }
    }
}
