using System;
using DG.Tweening; 
using UnityEngine;

public class CreateTileAction : BaseAction<CreateTileActionParameters>
{
    public Tween TileTween;

    public CreateTileAction(CreateTileActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.Context;
        var grid = action_context.GridSystem.GetGridObjectArray;
        var spawnY = action_context.GridSystem.ConvertGridPositionToWorldPosition(parameters.TargetGridObject.GetGridPosition).y + parameters.SpawnYOffset;

        var newProfile = action_context.MatchDetector.GetRandomValidMatch3Profile
        (
            action_context.Match3BlockProfileContainer.Match3BlockProfiles,
            grid,
            parameters.TargetGridPosition.X,
            parameters.TargetGridPosition.Y
        );

        if (!action_context.BlockVisualManager.TryEnableVisualByProfile(newProfile, grid[parameters.TargetGridPosition.X, parameters.TargetGridPosition.Y], action_context.GridSystem.ConvertGridPositionToWorldPosition, spawnY)) return;
        newProfile.Init();
        parameters.TargetGridObject.SetMatch3BlockProfile(newProfile);

        TileTween = action_context.BlockVisualManager.CreateVisualMoveTween
        (
            parameters.TargetGridObject, 
            parameters.TargetGridObject.GetWorldPosition(action_context.LevelGridData.GridCellWidth, 
            action_context.LevelGridData.GridCellHeight), 
            action_context.LevelGridData.VisualFallSpeed, 
            Ease.OutBounce, 
            .2f
        );

        CompleteAction();
    }
}
