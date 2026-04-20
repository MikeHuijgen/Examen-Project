using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchAction", menuName = "Scriptable Objects/MatchAction")]
public class MatchAction : Match3BaseAction
{
    public override IEnumerator Execute(IActionContext actionContext)
    {
        yield return null;
    }
}
