using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwapAction : BaseAction<SwapActionParameters>
{
    private bool _revertingSwap;
    public SwapAction(SwapActionParameters parameters, ActionContext context) : base(parameters, context){}

    public override void Execute(Action OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        HandleSwapLogic();
    }

    private bool CheckForMatch(out HashSet<Match> matches)
    {
        matches = action_context.MatchDetector.CheckForAllMatches
        (
            action_context.GridSystem.GetGridObjectArray,
            action_context.LevelGridData.GridWidth,
            action_context.LevelGridData.GridHeight
        );
        
        if (matches.Count <= 0) return false;
        
        return true;
    }

    private void HandleSwapLogic()
    {
        var from = parameters.from;
        var to = parameters.to;

        action_context.GridSystem.SwapGridObjectsData(from, to);
        action_context.BlockVisualManager.SwapVisuals
        (
            from, 
            to, 
            action_context.GridSystem.ConvertGridPositionToWorldPosition,
            action_context.LevelGridData.VisualSwapSpeed,
            Ease.InOutQuad,
            OnVisualSwapComplete
        );        
    }

    private void OnVisualSwapComplete()
    {
        if (_revertingSwap) return;

        if(!CheckForMatch(out var matches))
        {
            ReveredSwap();
            Debug.Log("Swap action done");
            CompleteAction();
            return;
        }

        action_context.GridActionProcessor.ProcessAction(new MatchAction(new MatchActionParameters{Matches = matches}, action_context));

        Debug.Log("Swap action done");
        CompleteAction();
    }

    private void ReveredSwap()
    {
        _revertingSwap = true;

        HandleSwapLogic();
    }
}
