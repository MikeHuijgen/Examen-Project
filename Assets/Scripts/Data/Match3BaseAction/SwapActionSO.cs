using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SwapAction", menuName = "Scriptable Objects/Match3/Actions/SwapAction")]
public class SwapActionSO : Match3BaseActionSO
{
    public static Action<IActionContext> PreActionEvent;
    public override IEnumerator Execute(IActionContext actionContext)
    {
        if (actionContext is not SwapActionContext) yield return null;
        var swapActionContext = CastContext(actionContext);
        swapActionContext.SwapGridObjectData(swapActionContext.From, swapActionContext.To);
        ExecuteSubActions();
    }

    private SwapActionContext CastContext(IActionContext context)
    {
        return (SwapActionContext)context;
    }

    public void CallPreActionEvent(IActionContext actionContext) => PreActionEvent?.Invoke(actionContext);
}
