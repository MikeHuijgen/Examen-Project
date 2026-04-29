using System;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public struct SwapActionParameters
{
    public GridObject from;
    public GridObject to;
    public float tweenSwapSpeed;
    public Action<GridObject, GridObject> dataSwapCallback;
    public Func<GridObject, GridObject, Func<GridPosition, Vector3>, float, Ease, Task> visualSwapCallback;
    public Func<GridPosition, Vector3> GetWorldPositionCallback;
}
