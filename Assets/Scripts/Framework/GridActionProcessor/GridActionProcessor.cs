using System;
using System.Collections.Generic;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour 
{
    public event Action OnParentActionComplete; 
    private Stack<BaseAction> _actionStack = new Stack<BaseAction>();

    public void ProcessAction<Tparameters>(BaseAction<Tparameters> action, Action<BaseAction> onComplete = null)
    {
        if(_actionStack.TryPeek(out var parentAction))
        {
            if (parentAction.IsCanceled) return;
            
            parentAction.SetActionState(BaseAction.ActionState.Waiting);
            action.Parent = parentAction;
            parentAction.ChainedActions.Add(action);
        }

        _actionStack.Push(action);

        action.SetActionState(BaseAction.ActionState.Running);
        ActionDebugRegistry.ActiveActions.Add(action);

        action.Execute(completedAction =>
        {
            OnActionComplete(completedAction);

            onComplete?.Invoke(completedAction);
        });
    }

    private void OnActionComplete(BaseAction source)
    {
        if (_actionStack.TryPeek(out var parentAction) && source == parentAction) OnParentActionComplete?.Invoke();
        source.SetActionState(BaseAction.ActionState.Completed);
        source.Parent?.SetActionState(BaseAction.ActionState.Running);
        _actionStack.Pop();
    }

    public void CancelCurrentChain()
    {
        if (!_actionStack.TryPeek(out var current)) return;

        current.Root.Cancel();

        _actionStack.Clear();
    }
}
