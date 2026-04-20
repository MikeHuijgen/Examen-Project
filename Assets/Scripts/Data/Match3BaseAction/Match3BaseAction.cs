using System.Collections;
using UnityEngine;

public abstract class Match3BaseAction : ScriptableObject
{
    public Match3BaseSubAction[] SubActions;
    public abstract IEnumerator Execute(IActionContext actionContext);

    protected void ExecuteSubActions()
    {
        foreach (var action in SubActions)
        {
            action.Execute();
        }
    }
}
