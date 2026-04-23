using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FallAction", menuName = "Scriptable Objects/FallAction")]
public class FallActionSO : Match3BaseActionSO
{
    public override void Execute()
    {
        ExecuteSubActions();
    }
}
