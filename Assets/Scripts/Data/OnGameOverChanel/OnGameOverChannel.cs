using System;
using UnityEngine;

// Ik heb het nu gefixt maar echt Keith fix je benamingen het is "RaiseEvent" niet "RaceEvent" en ook "Channel" schrijf je niet met één "n" we verkopen geen parfum
[CreateAssetMenu(fileName = "OnGameOverChanel", menuName = "Scriptable Objects/OnGameOverChanel")]
public class OnGameOverChannel : ScriptableObject 
{
    public event Action OnGameOver;

    public void RaiseEvent() => OnGameOver?.Invoke(); 
}

