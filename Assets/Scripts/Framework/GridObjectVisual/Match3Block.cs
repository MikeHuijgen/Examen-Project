using UnityEngine;
using System;

public class Match3Block : MonoBehaviour
{
    [SerializeField] private FakeAttack fakeAttack;

    public void SetPosition(Vector3 newPosition) => transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    public FakeAttack GetFakeAttack => fakeAttack;
}
