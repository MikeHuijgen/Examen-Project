using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SwapAction", menuName = "Scriptable Objects/Match3/Actions/SwapAction")]
public class SwapAction : Match3BaseAction
{
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
}
