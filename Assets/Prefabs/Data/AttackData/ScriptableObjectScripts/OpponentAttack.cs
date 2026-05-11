using UnityEngine;

[CreateAssetMenu(fileName = "Opponent", menuName = "Scriptable Objects/Opponent")]
public class OpponentAttack : BaseAttack
{
    [Range(0,2)]public int Direction;
    public AnimationClip ChargeAnimation;
    public float ChargeDurationTime;
}