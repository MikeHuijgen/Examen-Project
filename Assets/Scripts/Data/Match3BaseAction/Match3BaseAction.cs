using UnityEngine;

public abstract class Match3BaseAction : ScriptableObject
{
    public Match3BaseSubAction[] SubActions;
    public virtual void Execute() {ExecuteSubActions();}

    private void ExecuteSubActions()
    {
        foreach (var action in SubActions)
        {
            action.Execute();
        }
    }
}
