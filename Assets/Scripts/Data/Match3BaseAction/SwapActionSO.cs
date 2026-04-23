using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SwapAction", menuName = "Scriptable Objects/Match3/Actions/SwapAction")]
public class SwapActionSO : Match3BaseActionSO
{
    public override IEnumerator Execute()
    {
        ExecuteSubActions();
        yield return null;
    }
}
