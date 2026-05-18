using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ReshuffleAction : BaseAction<ReshuffleActionParameters>
{
    private GridObject[,] _grid;
    private float _spawnOffset = 5f;
    private Sequence _sequence;
    private List<Tween> _tweens;

    public ReshuffleAction(ReshuffleActionParameters parameters) : base(parameters) { }

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.actionContext;
        _grid = action_context.GridSystem.GetGridObjectArray;
        _tweens = new List<Tween>();
        ReshuffleGrid();
    }

    private void ReshuffleGrid()
    {
        _tweens.Clear();
        _sequence = DOTween.Sequence();

        foreach (var gridObject in _grid)
        {
            if (IsCanceled) break;
            action_context.BlockVisualManager.TryDisableVisualOnGridObject(gridObject);
            gridObject.SetMatch3BlockProfile(null);
        }

        for (var x = 0; x < action_context.LevelGridData.GridWidth; x++)
        {
            if (IsCanceled) break;
            for (int y = 0; y < action_context.LevelGridData.GridHeight; y++)
            {
                if (IsCanceled) break;
                var newTileAction = new CreateTileAction(new CreateTileActionParameters
                {
                    actionContext = action_context,
                    targetGridPosition = new GridPosition(x, y),
                    spawnYOffset = _spawnOffset,
                    targetGridObject = _grid[x, y]
                });

                action_context.GridActionProcessor.ProcessAction(newTileAction);

                if (newTileAction.TileTween == null) continue;

                _tweens.Add(newTileAction.TileTween);
            }
        }

        CheckForPossibleMoves();
    }

    private void CheckForPossibleMoves()
    {
        if (action_context.MatchDetector.PlayerHasPossibleMoves(_grid, action_context.GridSystem.SwapGridObjectsData))
        {
            foreach (var t in _tweens)
            {
                if (IsCanceled) break;
                _sequence.Join(t);
            }

            _sequence.OnComplete(() =>
            {
                CompleteAction();
            });
            return;
        }

        ReshuffleGrid();
    }

    public override void Cancel()
    {
        base.Cancel();

        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }
    }
}
