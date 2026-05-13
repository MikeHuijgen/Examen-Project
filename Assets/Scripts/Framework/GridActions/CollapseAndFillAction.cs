using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CollapseAndFillAction : BaseAction<CollapseAndFillActionParameters>
{
    private float _spawnOffset = 5f;
    private List<Tween> _tweens;

    public CollapseAndFillAction(CollapseAndFillActionParameters parameters, ActionContext context) : base(parameters, context){}

    public override void Execute(Action<BaseAction> onActionComplete)
    {
        Debug.Log("Started");
        on_action_complete = onActionComplete;
        _tweens = new List<Tween>();
        var grid = action_context.GridSystem.GetGridObjectArray;

        for (int x = 0; x < action_context.LevelGridData.GridWidth; x++)
        {
            HandleCollapse(grid, x);
            HandleFill(grid, x);
        }

        if (_tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in _tweens) seq.Join(t);

            Task.WhenAll(seq.AsyncWaitForCompletion());
        }

        CompleteAction();
    }

    private void HandleCollapse(GridObject[,] grid, int x)
    {
        var fallables = new List<(GridObject tile, Match3BlockProfile profile)>();

        for (var y = 0; y < action_context.LevelGridData.GridHeight; y++)
        {
            var tile = grid[x, y];
            var profile = tile.GetMatch3BlockProfile;

            if (profile == null || !profile.HasRule("CollapseAndFill")) continue;

            fallables.Add((tile, profile));
            tile.SetMatch3BlockProfile(null);
        }

        var fallIndex = 0;

        for (var y = 0; y < action_context.LevelGridData.GridHeight; y++)
        {
            var targetGridObject = grid[x, y];

            if (targetGridObject.GetMatch3BlockProfile != null) continue;

            if (fallIndex >= fallables.Count) break;

            var tile = fallables[fallIndex];

            targetGridObject.SetMatch3BlockProfile(tile.profile);

            action_context.BlockVisualManager.MoveVisualBinding(tile.tile, targetGridObject);

            var tween = action_context.BlockVisualManager.CreateVisualMoveTween(
                targetGridObject,
                targetGridObject.GetWorldPosition(action_context.LevelGridData.GridCellWidth, action_context.LevelGridData.GridCellHeight),
                action_context.LevelGridData.VisualFallSpeed,
                Ease.OutBounce,
                .2f
            );

            _tweens.Add(tween);

            fallIndex++;
        }
    }

    private void HandleFill(GridObject[,] grid, int x)
    {
        for (var y = 0; y < action_context.LevelGridData.GridHeight; y++)
        {
            var targetGrid = grid[x, y];
            if (targetGrid.GetMatch3BlockProfile != null) continue;

            var spawnY = action_context.GridSystem.ConvertGridPositionToWorldPosition(targetGrid.GetGridPosition).y + _spawnOffset;

            var newMatch3Profile = action_context.MatchDetector.GetRandomValidMatch3Profile(parameters.Match3BlockProfiles, grid, x, y);
            if (!action_context.BlockVisualManager.TryEnableVisualByProfile(newMatch3Profile, grid[x, y], action_context.GridSystem.ConvertGridPositionToWorldPosition, spawnY)) continue;
            newMatch3Profile.Init();
            targetGrid.SetMatch3BlockProfile(newMatch3Profile);

            var tween = action_context.BlockVisualManager.CreateVisualMoveTween(targetGrid, targetGrid.GetWorldPosition(action_context.LevelGridData.GridCellWidth, action_context.LevelGridData.GridCellHeight), action_context.LevelGridData.VisualFallSpeed, Ease.OutBounce, .2f);
            _tweens.Add(tween);
        }
    }
}
