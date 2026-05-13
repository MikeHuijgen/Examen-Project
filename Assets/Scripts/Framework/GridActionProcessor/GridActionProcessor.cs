using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour 
{
    private Stack<BaseAction> _actionStack = new Stack<BaseAction>();

    public void ProcessAction<Tparameters>(BaseAction<Tparameters> action)
    {
        if(_actionStack.TryPeek(out var parentAction))
        {
            parentAction.SetActionState(BaseAction.ActionState.Waiting);
        }

        _actionStack.Push(action);

        action.SetActionState(BaseAction.ActionState.Running);
        ActionDebugRegistry.ActiveActions.Add(action);

        action.Execute(OnActionComplete);
    }

    private void OnActionComplete(BaseAction source)
    {
        source.SetActionState(BaseAction.ActionState.Completed);
        _actionStack.Pop();
        if (_actionStack.Count <= 0) return;

        if(_actionStack.TryPeek(out var parentAction))
        {
            parentAction.SetActionState(BaseAction.ActionState.Running);
        }   
    }
}
