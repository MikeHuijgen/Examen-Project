using System;
using UnityEngine;
using UnityEngine.Events;

public class EventOnStart : MonoBehaviour
{
    public UnityEvent EventToInvoke;

    private void Start() => EventToInvoke?.Invoke();
}
