using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OnGameOverChanel", menuName = "Scriptable Objects/OnGameOverChanel")]
public class OnGameOverChanel : ScriptableObject
{
    public event Action OnGameOver;

    public void RaceEvent() => OnGameOver?.Invoke();
}
