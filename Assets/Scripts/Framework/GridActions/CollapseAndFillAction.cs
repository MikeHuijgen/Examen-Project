using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CollapseAndFillAction : BaseAction<CollapseAndFillActionParameters>
{
    private float _spawnOffset = 5f;
    private List<Tween> _tweens;
    private int _gridHeight;
    private int _gridWidth;
    private GridObject[,] _grid;


    public CollapseAndFillAction(CollapseAndFillActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> onActionComplete)
    {
        on_action_complete = onActionComplete;
        action_context = parameters.actionContext;

        _gridHeight = action_context.LevelGridData.GridHeight;
        _gridWidth = action_context.LevelGridData.GridWidth;
        _grid = action_context.GridSystem.GetGridObjectArray;
        

        _tweens = new List<Tween>();

        for (int x = 0; x < _gridWidth; x++)
        {
            HandleCollapse(x);
            HandleFill(x);
        }

        if (_tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in _tweens) seq.Join(t);

            seq.OnComplete(() => CheckForNewMatchAfterCollapse());
        }
        else
        {
            CheckForNewMatchAfterCollapse();
        }
    }

    private void HandleCollapse(int x)
    {
        var fallables = new List<(GridObject tile, Match3BlockProfile profile)>();

        for (var y = 0; y < _gridHeight; y++)
        {
            var tile = _grid[x, y];
            var profile = tile.GetMatch3BlockProfile;

            if (profile == null || !profile.HasRule("CollapseAndFill")) continue;

            fallables.Add((tile, profile));
            tile.SetMatch3BlockProfile(null);
        }

        var fallIndex = 0;

        for (var y = 0; y < _gridHeight; y++)
        {
            var targetGridObject = _grid[x, y];

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

    private void HandleFill(int x)
    {
        for (var y = 0; y < _gridHeight; y++)
        {
            var targetGrid = _grid[x, y];
            if (targetGrid.GetMatch3BlockProfile != null) continue;

            var newTileAction = new CreateTileAction(new CreateTileActionParameters
            {
                actionContext = action_context,
                targetGridPosition = new GridPosition(x, y),
                spawnYOffset = _spawnOffset,
                targetGridObject = targetGrid
            });

            action_context.GridActionProcessor.ProcessAction(newTileAction);

            if (newTileAction.TileTween == null) continue;

            _tweens.Add(newTileAction.TileTween);
        }
    }

    private void CheckForNewMatchAfterCollapse()
    {
        var matches = action_context.MatchDetector.CheckForAllMatches(_grid, _gridWidth, _gridHeight);
        if (matches.Count <= 0)
        {
            CompleteAction();
            return;
        }

        action_context.GridActionProcessor.ProcessAction(new MatchAction(new MatchActionParameters{Matches = matches, actionContext = action_context}), _ => {CompleteAction();});
    }
}
