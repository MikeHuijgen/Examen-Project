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
    private Sequence _sequence;


    public CollapseAndFillAction(CollapseAndFillActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> onActionComplete)
    {
        on_action_complete = onActionComplete;
        action_context = parameters.Context;

        _gridHeight = action_context.LevelGridData.GridHeight;
        _gridWidth = action_context.LevelGridData.GridWidth;
        _grid = action_context.GridSystem.GetGridObjectArray;


        _tweens = new List<Tween>();

        for (int x = 0; x < _gridWidth; x++)
        {
            if (IsCanceled) break;

            HandleCollapse(x);
            HandleFill(x);
        }

        _sequence = DOTween.Sequence();

        if (_tweens.Count <= 0)
        {
            CheckForNewMatchAfterCollapse();
            return;
        }

        foreach (var t in _tweens)
        {
            if (IsCanceled) break;
            _sequence.Join(t);
        }

        _sequence.OnComplete(() =>
        {
            if (IsCanceled) return;
            CheckForNewMatchAfterCollapse();
        });

        _sequence.Play();
    }

    private void HandleCollapse(int x)
    {
        var fallables = new List<(GridObject tile, Match3BlockProfile profile)>();

        for (var y = 0; y < _gridHeight; y++)
        {
            if (IsCanceled) break;
            var tile = _grid[x, y];
            var profile = tile.GetMatch3BlockProfile;

            if (profile == null || !profile.HasRule("CollapseAndFill")) continue;

            fallables.Add((tile, profile));
            tile.SetMatch3BlockProfile(null);
        }

        var fallIndex = 0;

        for (var y = 0; y < _gridHeight; y++)
        {
            if (IsCanceled) break;
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
            if (IsCanceled) break;
            var targetGrid = _grid[x, y];
            if (targetGrid.GetMatch3BlockProfile != null) continue;

            var newTileAction = new CreateTileAction(new CreateTileActionParameters
            {
                Context = action_context,
                TargetGridPosition = new GridPosition(x, y),
                SpawnYOffset = _spawnOffset,
                TargetGridObject = targetGrid
            });

            action_context.GridActionProcessor.ProcessAction(newTileAction);

            if (newTileAction.TileTween == null) continue;

            _tweens.Add(newTileAction.TileTween);
        }
    }

    private void CheckForNewMatchAfterCollapse()
    {
        if (IsCanceled) return;
        var matches = action_context.MatchDetector.CheckForAllMatches(_grid, _gridWidth, _gridHeight);
        if (matches.Count <= 0)
        {
            CompleteAction();
            return;
        }

        action_context.GridActionProcessor.ProcessAction(new MatchAction(new MatchActionParameters { Matches = matches, Context = action_context }), _ => { CompleteAction(); });
    }

    public override void Cancel()
    {
        base.Cancel();

        if (_sequence != null && _sequence.IsActive()) _sequence.Kill();
    }
}
