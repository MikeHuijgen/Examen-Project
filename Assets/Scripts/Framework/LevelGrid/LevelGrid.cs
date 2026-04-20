using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.right,
        Vector2Int.up
    };

    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private Match3BlockProfileContainer match3BlockProfileContainer;
    [SerializeField] private BlockVisualManager blockVisualManager;
    private GridSystem _gridSystem;
    private MatchDetector _matchDetector;
    private GridHit _beginTouchGridPosition;
    private GridHit? _currentSelectedGridPosition;
    private List<Tween> _tweens = new List<Tween>();
    private List<GridObject> _tiles = new List<GridObject>();
    public static event Action<BaseAttack> OnMatchDestroyed;
    private bool _allowInput = true;


    private void Awake()
    {
        _gridSystem = new GridSystem(
            levelGridData.GridWidth,
            levelGridData.GridHeight,
            levelGridData.GridCellWidth,
            levelGridData.GridCellHeight);

        _matchDetector = new MatchDetector();

        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridTileVisuals(levelGridData.GridTileVisual);
        ReshuffleGrid();

        CharacterInput.Instance.OnNewFingerDownInput += OnNewFingerDownInput;
        CharacterInput.Instance.OnNewFingerUpInput += OnNewFingerUpInput;
    }

    private void OnNewFingerDownInput(Vector2 fingerPosition)
    {
        var newGridHit = _gridSystem.ConvertScreenPositionToGridHit(fingerPosition);

        _beginTouchGridPosition = newGridHit;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        if (!_allowInput) return;
        var newGridHit = _gridSystem.ConvertScreenPositionToGridHit(fingerPosition);


        var endTouchGridPosition = newGridHit;

        if (endTouchGridPosition.hitGridPosition == _beginTouchGridPosition.hitGridPosition && !_currentSelectedGridPosition.HasValue)
        {
            _currentSelectedGridPosition = _beginTouchGridPosition;
            _gridSystem.SelectTileByGridPosition(_currentSelectedGridPosition.Value.hitGridPosition);
            return;
        }

        var isClickMove = _beginTouchGridPosition.hitGridPosition == endTouchGridPosition.hitGridPosition;

        if (isClickMove)
        {
            var endGridPosition = _gridSystem.CalculateClickedEndGridPosition(_currentSelectedGridPosition.Value.hitGridPosition, endTouchGridPosition.rawX, endTouchGridPosition.rawY, levelGridData.ClickTolerance);

            endGridPosition = _gridSystem.CheckGridBounds(endGridPosition);

            if (endGridPosition == _currentSelectedGridPosition.Value.hitGridPosition)
            {
                ResetCurrentGridPosition();
                return;
            }

            if (_gridSystem.IsDiagonalMove(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition))
            {
                ResetCurrentGridPosition();
                return;
            }


            StartCoroutine(HandleMove(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition));
        }
        else
        {
            var endGridPosition = _gridSystem.CalculateSwipeEndGridPosition(_beginTouchGridPosition, endTouchGridPosition, levelGridData.SwipeDirectionTolerance, levelGridData.SwipeMaxDiagonalDeviation);

            endGridPosition = _gridSystem.CheckGridBounds(endGridPosition);

            StartCoroutine(HandleMove(_beginTouchGridPosition.hitGridPosition, endGridPosition));
        }

        _currentSelectedGridPosition = null;
    }

    private void ResetCurrentGridPosition()
    {
        _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.hitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private IEnumerator HandleMove(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        if (_currentSelectedGridPosition != null) _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.hitGridPosition);

        _allowInput = false;

        var beginGridObject = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var endGridObject = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        if (beginGridObject == null || endGridObject == null || beginGridObject == endGridObject)
        {
            _allowInput = true;
            yield break;
        }

        if (!beginGridObject.GetMatch3BlockProfile.HasAction<SwapAction>(out var beginSwapAction) || !endGridObject.GetMatch3BlockProfile.HasAction<SwapAction>(out var endSwapAction)) 
        {
            _allowInput = true;
            yield break;
        }

        beginSwapAction.Execute(new SwapActionContext(beginGridObject, endGridObject, _gridSystem.SwapGridObjectsData));
        yield return endSwapAction.Execute(new SwapActionContext(endGridObject, beginGridObject, _gridSystem.SwapGridObjectsData));

        yield return MoveVisuals(beginGridObject, endGridObject);

        var matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        if (!HasAMatch(matches))
        {
            beginSwapAction.Execute(new SwapActionContext(beginGridObject, endGridObject, _gridSystem.SwapGridObjectsData));
            yield return endSwapAction.Execute(new SwapActionContext(endGridObject, beginGridObject, _gridSystem.SwapGridObjectsData));

            yield return MoveVisuals(beginGridObject, endGridObject);
            _allowInput = true;
            yield break;
        }

        while (true)
        {
            matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

            if (!HasAMatch(matches)) break;

            yield return DestroyMatches(matches);
            yield return CollapseAndFill();
        }

        CheckForPossibleMoves();

        _allowInput = true;
    }

    private void CheckForPossibleMoves()
    {
        var grid = _gridSystem.GetGridObjectArray;
        if (PlayerHasPossibleMoves(grid)) return;

        foreach (var gridObject in grid)
        {
            blockVisualManager.TryDisableVisualOnGridObject(gridObject);
            gridObject.SetMatch3BlockProfile(null);
        }

        ReshuffleGrid();
    }

    private bool HasAMatch(HashSet<Match> matchList)
    {
        if (matchList.Count == 0) return false;

        return true;
    }

    private IEnumerator MoveVisuals(GridObject beginGridObject, GridObject endGridObject)
    {
        var newBeginGridObjectPosition = _gridSystem.ConvertGridPositionToWorldPosition(beginGridObject.GetGridPosition);
        var newEndGridObjectPosition = _gridSystem.ConvertGridPositionToWorldPosition(endGridObject.GetGridPosition);

        StartCoroutine(blockVisualManager.MoveVisualWithTweenRoutine(beginGridObject, newBeginGridObjectPosition, levelGridData.VisualSwapSpeed, Ease.InOutQuad));
        yield return blockVisualManager.MoveVisualWithTweenRoutine(endGridObject, newEndGridObjectPosition, levelGridData.VisualSwapSpeed, Ease.InOutQuad);
    }

    private IEnumerator DestroyMatches(HashSet<Match> matches)
    {
        // Uiteindelijk even kijken of de data set niet beter kan in een destroy action
        var grid = _gridSystem.GetGridObjectArray;
        foreach (var match in matches)
        {
            for (int i = 0; i < match.MatchedObjectGroup.Length; i++)
            {
                var gridPos = new GridPosition(match.MatchedObjectGroup[i].GetGridPosition.X, match.MatchedObjectGroup[i].GetGridPosition.Y);
                grid[gridPos.X, gridPos.Y].SetMatch3BlockProfile(null);
            }
        }

        yield return blockVisualManager.DestroyMatches(matches);
    }

    public void ReshuffleGrid()
    {
        var grid = _gridSystem.GetGridObjectArray;
        for (var x = 0; x < levelGridData.GridWidth; x++)
        {
            for (int y = 0; y < levelGridData.GridHeight; y++)
            {
                var newMatch3Profile = _matchDetector.GetRandomValidMatch3Profile(match3BlockProfileContainer.match3BlockProfiles, grid, x, y);
                if (!blockVisualManager.TryEnableVisualByProfile(newMatch3Profile, grid[x, y], _gridSystem.ConvertGridPositionToWorldPosition)) continue;
                newMatch3Profile.Init();
                grid[x, y].SetMatch3BlockProfile(newMatch3Profile);
            }
        }
    }

    private IEnumerator CollapseAndFill()
    {
        var grid = _gridSystem.GetGridObjectArray;
        _tweens.Clear();

        for (int x = 0; x < levelGridData.GridWidth; x++)
        {
            var writeY = 0;
            _tiles.Clear();

            for (var y = 0; y < levelGridData.GridHeight; y++)
            {
                var match3BlockProfile = grid[x, y].GetMatch3BlockProfile;
                if (match3BlockProfile == null) continue;

                _tiles.Add(grid[x, y]);
            }

            foreach (var tile in _tiles)
            {
                var targetGrid = grid[x, writeY];

                targetGrid.SetMatch3BlockProfile(tile.GetMatch3BlockProfile);
                blockVisualManager.MoveVisualBinding(tile, targetGrid);
                var tween = blockVisualManager.CreateVisualMoveTween(targetGrid, targetGrid.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight), levelGridData.VisualFallSpeed, Ease.OutBounce, .7f);
                _tweens.Add(tween);

                writeY++;
            }

            var spawnCount = levelGridData.GridHeight - writeY;

            for (var i = 0; i < spawnCount; i++)
            {
                var y = writeY + i;

                var targetGrid = grid[x, y];

                var spawnY = _gridSystem.ConvertGridPositionToWorldPosition(targetGrid.GetGridPosition).y + (spawnCount - i) + 5f;

                var newMatch3Profile = _matchDetector.GetRandomValidMatch3Profile(match3BlockProfileContainer.match3BlockProfiles, grid, x, y);
                if (!blockVisualManager.TryEnableVisualByProfile(newMatch3Profile, grid[x, y], _gridSystem.ConvertGridPositionToWorldPosition, spawnY)) continue;

                targetGrid.SetMatch3BlockProfile(newMatch3Profile);

                var tween = blockVisualManager.CreateVisualMoveTween(targetGrid, targetGrid.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight), levelGridData.VisualFallSpeed, Ease.OutBounce, .7f);
                _tweens.Add(tween);
            }
        }

        if (_tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in _tweens) seq.Join(t);

            yield return seq.WaitForCompletion();
        }
    }

    private bool PlayerHasPossibleMoves(GridObject[,] grid)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                foreach (var dir in Directions)
                {
                    var nx = x + dir.x;
                    var ny = y + dir.y;

                    if (nx >= width || ny >= height) continue;

                    _gridSystem.SwapGridObjectsData(grid[x, y], grid[nx, ny]);

                    if (_matchDetector.HasMatchAt(grid, x, y) || _matchDetector.HasMatchAt(grid, nx, ny))
                    {
                        _gridSystem.SwapGridObjectsData(grid[x, y], grid[nx, ny]);
                        return true;
                    }

                    _gridSystem.SwapGridObjectsData(grid[x, y], grid[nx, ny]);
                }
            }
        }
        return false;
    }

}
