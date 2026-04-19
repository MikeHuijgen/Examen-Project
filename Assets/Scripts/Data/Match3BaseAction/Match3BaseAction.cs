using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockBaseAction", menuName = "Scriptable Objects/Match3/Blocks/Actions/Action")]
public class Match3BaseAction : ScriptableObject
{
    public Match3BaseSubAction[] SubActions;
    public void Execute()
    {
        ExecuteSubActions();
    }

    private void ExecuteSubActions()
    {
        foreach (var action in SubActions)
        {
            action.Execute();
        }
    }
}
