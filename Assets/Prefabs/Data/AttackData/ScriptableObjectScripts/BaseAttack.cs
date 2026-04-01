using UnityEngine;

[CreateAssetMenu(fileName = "Base", menuName = "Scriptable Objects/Base")]
public class BaseAttack : ScriptableObject
{
    public float Damage;
    public Animation AttackAnim;
    public float AttackDurationTime;
}
