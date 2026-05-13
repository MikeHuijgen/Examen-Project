using System;
using System.Collections.Generic;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour 
{
    private Stack<BaseAction> _actionStack = new Stack<BaseAction>();

    public void ProcessAction<Tparameters>(BaseAction<Tparameters> action, Action<BaseAction> onComplete = null)
    {
        if(_actionStack.TryPeek(out var parentAction))
        {
            parentAction.SetActionState(BaseAction.ActionState.Waiting);
            action.parent = parentAction;
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
        source.SetActionState(BaseAction.ActionState.Completed);
        source.parent?.SetActionState(BaseAction.ActionState.Running);
        _actionStack.Pop();
    }
}
