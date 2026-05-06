using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CollapseAndFillAction : BaseAction<CollapseAndFillActionParameters>
{
    private float _spawnOffset = 5f;
    private List<Tween> _tweens;

    public CollapseAndFillAction(CollapseAndFillActionParameters parameters) : base(parameters) { }

    public override async Task Execute()
    {
        _tweens = new List<Tween>();
        var grid = parameters.Grid;

        for (int x = 0; x < parameters.GridWidth; x++)
        {
            HandleCollapse(grid, x);
            HandleFill(grid, x);
        }

        if (_tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in _tweens) seq.Join(t);

            await Task.WhenAll(seq.AsyncWaitForCompletion());
        }
    }

    private void HandleCollapse(GridObject[,] grid, int x)
    {
        var fallables = new List<(GridObject tile, Match3BlockProfile profile)>();

        for (var y = 0; y < parameters.GridHeight; y++)
        {
            var tile = grid[x, y];
            var profile = tile.GetMatch3BlockProfile;

            if (profile == null || !profile.HasRule("CollapseAndFill")) continue;

            fallables.Add((tile, profile));
            tile.SetMatch3BlockProfile(null);
        }

        var fallIndex = 0;

        for (var y = 0; y < parameters.GridHeight; y++)
        {
            var targetGridObject = grid[x, y];

            if (targetGridObject.GetMatch3BlockProfile != null) continue;

            if (fallIndex >= fallables.Count) break;

            var tile = fallables[fallIndex];

            targetGridObject.SetMatch3BlockProfile(tile.profile);

            parameters.MoveVisualBindingCallback(tile.tile, targetGridObject);

            var tween = parameters.CreateVisualMoveTweenCallback(
                targetGridObject,
                targetGridObject.GetWorldPosition(parameters.GridCellWidth, parameters.GridCellHeight),
                parameters.VisualFallSpeed,
                Ease.OutBounce,
                .2f
            );

            _tweens.Add(tween);

            fallIndex++;
        }
    }

    private void HandleFill(GridObject[,] grid, int x)
    {
        for (var y = 0; y < parameters.GridHeight; y++)
        {
            var targetGrid = grid[x, y];
            if (targetGrid.GetMatch3BlockProfile != null) continue;

            var spawnY = parameters.GetWorldPositionCallback(targetGrid.GetGridPosition).y + _spawnOffset;

            var newMatch3Profile = parameters.GetRandomValidMatch3BlockCallBack(parameters.Match3BlockProfiles, grid, x, y);
            if (!parameters.TryEnableVisualByProfileCallback(newMatch3Profile, grid[x, y], parameters.GetWorldPositionCallback, spawnY)) continue;
            newMatch3Profile.Init();
            targetGrid.SetMatch3BlockProfile(newMatch3Profile);

            var tween = parameters.CreateVisualMoveTweenCallback(targetGrid, targetGrid.GetWorldPosition(parameters.GridCellWidth, parameters.GridCellHeight), parameters.VisualFallSpeed, Ease.OutBounce, .2f);
            _tweens.Add(tween);
        }
    }
}
