using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfile", menuName = "Scriptable Objects/Match3/Blocks/Profile")]
public class Match3BlockProfile : ScriptableObject
{
    public Match3BaseAction[] Actions;

    public bool HasAction<T>() where T : Match3BaseAction
    {
        foreach (var action in Actions)
        {
            if (action is T) return false;
        }

        return true;
    }
}
