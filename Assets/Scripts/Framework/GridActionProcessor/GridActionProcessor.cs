using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour 
{
    private Stack<BaseAction> _actionStack = new Stack<BaseAction>();

    public void ProcessAction<Tparameters>(BaseAction<Tparameters> action)
    {
        _actionStack.Push(action);

        action.Execute(OnActionComplete);
    }

    private void OnActionComplete()
    {
        _actionStack.Pop();
    }
}
