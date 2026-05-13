using UnityEngine;
using System;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetBoolFalse(string name) => animator.SetBool(name, false);
}
