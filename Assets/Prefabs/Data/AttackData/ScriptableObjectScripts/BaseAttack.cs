using UnityEngine;

[CreateAssetMenu(fileName = "Base", menuName = "Scriptable Objects/Base")]
public class BaseAttack : ScriptableObject
{
    public int Damage;
    public AnimationClip AttackAnim;
    public float AttackDurationTime;
}
