using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfile", menuName = "Scriptable Objects/Match3/Profile")]
public class Match3BlockProfile : ScriptableObject
{
    public BaseAttack Attack;
    public RuleFlag[] ruleFlags;
}
