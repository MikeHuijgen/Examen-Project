using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwapAction : BaseAction<SwapActionParameters>
{
    public SwapAction(SwapActionParameters parameters) : base(parameters){}

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.actionContext;
        HandleSwapLogic();
    }

    private void HandleSwapLogic()
    {
        if(IsCanceled) return;
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

    private void HandleReverseSwapLogic()
    {
        if(IsCanceled) return;
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
            OnReveredVisualSwapComplete
        );   
    }

    private void OnVisualSwapComplete()
    {
        if(IsCanceled) return;

        var matches = action_context.MatchDetector.CheckForAllMatches
        (
            action_context.GridSystem.GetGridObjectArray,
            action_context.LevelGridData.GridWidth,
            action_context.LevelGridData.GridHeight
        );
        if(matches.Count <= 0)
        {
            HandleReverseSwapLogic();
            return;
        }

        action_context.GridActionProcessor.ProcessAction(new MatchAction(new MatchActionParameters{Matches = matches, actionContext = action_context}), _ => {CompleteAction();});
    }

    private void OnReveredVisualSwapComplete() => CompleteAction();
}
