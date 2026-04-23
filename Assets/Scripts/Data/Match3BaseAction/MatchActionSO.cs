using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchAction", menuName = "Scriptable Objects/MatchAction")]
public class MatchActionSO : Match3BaseActionSO
{
    public override IEnumerator Execute()
    {
        ExecuteSubActions();
        yield return null;
    }
}
