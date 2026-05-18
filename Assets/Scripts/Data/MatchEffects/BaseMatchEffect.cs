using Unity.Mathematics;
using UnityEngine;

public abstract class BaseMatchEffect : ScriptableObject
{
    public ParticleSystem BreakVFX;
    public abstract void ActivateEffect();
    public void PlayBreakEffectOnPosition(Vector3 targetPosition) => Instantiate(BreakVFX, targetPosition, quaternion.identity).Play();
}
