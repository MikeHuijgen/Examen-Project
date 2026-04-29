using System;
using UnityEngine;
using DG.Tweening;

public struct CollapseAndFillActionParameters
{
    public GridObject[,] Grid;
    public Match3BlockProfile[] Match3BlockProfiles;
    public float VisualFallSpeed;
    public int GridWidth;
    public int GridHeight;
    public float GridCellWidth;
    public float GridCellHeight;
    public Func<GridPosition, Vector3> GetWorldPositionCallback;
    public Func<GridObject, Vector3, float, Ease, float, Tween> CreateVisualMoveTweenCallback;
    public Func<Match3BlockProfile[], GridObject[,], int , int, Match3BlockProfile> GetRandomValidMatch3BlockCallBack;
    public Func<Match3BlockProfile, GridObject, Func<GridPosition, Vector3>, float, bool> TryEnableVisualByProfileCallback;
    public Action<GridObject, GridObject> MoveVisualBindingCallback;
}
