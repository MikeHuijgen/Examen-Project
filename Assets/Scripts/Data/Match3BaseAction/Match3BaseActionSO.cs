using System;
using System.Collections;
using UnityEngine;

public abstract class Match3BaseActionSO : ScriptableObject
{
    public Match3BaseSubAction[] SubActions;
    public abstract IEnumerator Execute();

    protected void ExecuteSubActions()
    {
        foreach (var action in SubActions)
        {
            action.Execute();
        }
    }
}
