using System;
using DG.Tweening; 

public class CreateTileAction : BaseAction<CreateTileActionParameters>
{
    public Tween TileTween;

    public CreateTileAction(CreateTileActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.actionContext;
        var grid = action_context.GridSystem.GetGridObjectArray;
        var spawnY = action_context.GridSystem.ConvertGridPositionToWorldPosition(parameters.targetGridObject.GetGridPosition).y + parameters.spawnYOffset;

        var newProfile = action_context.MatchDetector.GetRandomValidMatch3Profile
        (
            action_context.Match3BlockProfileContainer.match3BlockProfiles,
            grid,
            parameters.targetGridPosition.X,
            parameters.targetGridPosition.Y
        );

        if (!action_context.BlockVisualManager.TryEnableVisualByProfile(newProfile, grid[parameters.targetGridPosition.X, parameters.targetGridPosition.Y], action_context.GridSystem.ConvertGridPositionToWorldPosition, spawnY)) return;
        newProfile.Init();
        parameters.targetGridObject.SetMatch3BlockProfile(newProfile);

        TileTween = action_context.BlockVisualManager.CreateVisualMoveTween
        (
            parameters.targetGridObject, 
            parameters.targetGridObject.GetWorldPosition(action_context.LevelGridData.GridCellWidth, 
            action_context.LevelGridData.GridCellHeight), 
            action_context.LevelGridData.VisualFallSpeed, 
            Ease.OutBounce, 
            .2f
        );

        CompleteAction();
    }
}
