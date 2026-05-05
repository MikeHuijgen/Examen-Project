using UnityEngine;

[CreateAssetMenu(fileName = "AttackEnergyBar", menuName = "Scriptable Objects/AttackEnergyBar")]
public class AttackEnergyBar : ScriptableObject
{
    public float max;
    public float decayRate;
    public float gainPerAttack;
}
