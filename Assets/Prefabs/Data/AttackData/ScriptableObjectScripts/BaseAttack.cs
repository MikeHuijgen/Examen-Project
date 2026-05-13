using UnityEngine;

[CreateAssetMenu(fileName = "Base", menuName = "Scriptable Objects/Base")]
public class BaseAttack : ScriptableObject
{
    public float Damage;
    public AnimationClip AttackAnim;
    public float AttackDurationTime;
}
