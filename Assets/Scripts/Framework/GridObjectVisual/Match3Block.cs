using UnityEngine;
using System;

public class Match3Block : MonoBehaviour
{
    [SerializeField] private BaseAttack attackData;

    public void SetPosition(Vector3 newPosition) => transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    public BaseAttack GetAttackData => attackData;
}
