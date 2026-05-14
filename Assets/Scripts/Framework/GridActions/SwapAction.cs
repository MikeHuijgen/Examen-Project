using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwapAction : BaseAction<SwapActionParameters>
{
    private Sequence _sequence;

    public SwapAction(SwapActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.actionContext;

        if (IsCanceled) return;

        _sequence = DOTween.Sequence();

        HandleForwardSwap();
    }

    private void HandleForwardSwap()
    {
        var from = parameters.from;
        var to = parameters.to;

        action_context.GridSystem.SwapGridObjectsData(from, to);

        var tweens = action_context.BlockVisualManager.SwapVisualTweens(
            from,
            to,
            action_context.GridSystem.ConvertGridPositionToWorldPosition,
            action_context.LevelGridData.VisualSwapSpeed,
            Ease.InOutQuad
        );

        foreach (var t in tweens)
        {
            if (IsCanceled) break;
            _sequence.Join(t);
        }

        _sequence.AppendCallback(OnForwardSwapComplete);
    }

    private void OnForwardSwapComplete()
    {
        if (IsCanceled) return;

        var matches = action_context.MatchDetector.CheckForAllMatches(
            action_context.GridSystem.GetGridObjectArray,
            action_context.LevelGridData.GridWidth,
            action_context.LevelGridData.GridHeight
        );

        if (matches.Count > 0)
        {
            action_context.GridActionProcessor.ProcessAction(
                new MatchAction(new MatchActionParameters
                {
                    Matches = matches,
                    actionContext = action_context
                }),
                _ => CompleteAction()
            );

            return;
        }

        HandleReverseSwap();
    }

    private void HandleReverseSwap()
    {
        if (IsCanceled) return;

        var from = parameters.from;
        var to = parameters.to;

        action_context.GridSystem.SwapGridObjectsData(from, to);

        var tweens = action_context.BlockVisualManager.SwapVisualTweens(
            from,
            to,
            action_context.GridSystem.ConvertGridPositionToWorldPosition,
            action_context.LevelGridData.VisualSwapSpeed,
            Ease.InOutQuad
        );

        var reverseSequence = DOTween.Sequence();

        foreach (var t in tweens)
        {
            if (IsCanceled) break;
            reverseSequence.Join(t);
        }

        reverseSequence.OnComplete(OnReverseSwapComplete);

        _sequence = reverseSequence;
    }

    private void OnReverseSwapComplete()
    {
        if (IsCanceled) return;
        CompleteAction();
    }

    public override void Cancel()
    {
        base.Cancel();

        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }
    }
}
