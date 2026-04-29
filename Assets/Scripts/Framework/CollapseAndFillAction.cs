using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CollapseAndFillAction : BaseAction<CollapseAndFillActionParameters>
{
    private List<Tween> _tweens;
    private List<GridObject> _tiles;

    public CollapseAndFillAction(CollapseAndFillActionParameters parameters) : base(parameters){}

    public override async Task Execute()
    {
        _tweens = new List<Tween>();
        _tiles = new List<GridObject>();
        var grid = parameters.Grid;

        for (int x = 0; x < parameters.GridWidth; x++)
        {
            var writeY = 0;
            _tiles.Clear();

            for (var y = 0; y < parameters.GridHeight; y++)
            {
                var match3BlockProfile = grid[x, y].GetMatch3BlockProfile;
                if (match3BlockProfile == null) continue;

                _tiles.Add(grid[x, y]);
            }

            foreach (var tile in _tiles)
            {
                var targetGrid = grid[x, writeY];

                targetGrid.SetMatch3BlockProfile(tile.GetMatch3BlockProfile);
                parameters.MoveVisualBindingCallback(tile, targetGrid);
                var tween = parameters.CreateVisualMoveTweenCallback(targetGrid, targetGrid.GetWorldPosition(parameters.GridCellWidth, parameters.GridCellHeight), parameters.VisualFallSpeed, Ease.OutBounce, .7f);
                _tweens.Add(tween);

                writeY++;
            }

            var spawnCount = parameters.GridHeight - writeY;

            for (var i = 0; i < spawnCount; i++)
            {
                var y = writeY + i;

                var targetGrid = grid[x, y];

                var spawnY = parameters.GetWorldPositionCallback(targetGrid.GetGridPosition).y + (spawnCount - i) + 5f;

                var newMatch3Profile = parameters.GetRandomValidMatch3BlockCallBack(parameters.Match3BlockProfiles, grid, x, y);
                if (!parameters.TryEnableVisualByProfileCallback(newMatch3Profile, grid[x, y], parameters.GetWorldPositionCallback, spawnY)) continue;
                newMatch3Profile.Init();
                targetGrid.SetMatch3BlockProfile(newMatch3Profile);

                var tween = parameters.CreateVisualMoveTweenCallback(targetGrid, targetGrid.GetWorldPosition(parameters.GridCellWidth, parameters.GridCellHeight), parameters.VisualFallSpeed, Ease.OutBounce, .7f);
                _tweens.Add(tween);
            }
        }

        if (_tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in _tweens) seq.Join(t);

            await Task.WhenAll(seq.AsyncWaitForCompletion());
        }
    }
}
