using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActionProcessor
{
    public event Action<IActionContext> OnPreActionEvent;
    public Func<IActionContext, Task> OnAfterActionEvent;

    public async Task StartActionProcess(IActionContext actionContext, Match3BaseActionSO actionSO)
    {
        OnPreActionEvent?.Invoke(actionContext);

        actionSO.Execute(actionContext);

        if (OnAfterActionEvent == null) return;

        var tasks = OnAfterActionEvent.GetInvocationList().Cast<Func<IActionContext, Task>>().Select(f => f(actionContext));

        await Task.WhenAll(tasks);
    } 
}
