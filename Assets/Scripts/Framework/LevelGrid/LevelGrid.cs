using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private Transform tileVisualHolder;
    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private Match3BlockProfileContainer match3BlockProfileContainer;
    [SerializeField] private BlockVisualManager blockVisualManager;
    [SerializeField] private GridActionProcessor gridActionProcessor;
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
        _matchDetector = new MatchDetector();
        _gridSystem = new GridSystem(
            levelGridData.GridWidth,
            levelGridData.GridHeight,
            levelGridData.GridCellWidth,
            levelGridData.GridCellHeight);

        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridTileVisuals(levelGridData.GridTileVisual, tileVisualHolder);
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


            HandleMove(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition);
        }
        else
        {
            var endGridPosition = _gridSystem.CalculateSwipeEndGridPosition(_beginTouchGridPosition, endTouchGridPosition, levelGridData.SwipeDirectionTolerance, levelGridData.SwipeMaxDiagonalDeviation);

            endGridPosition = _gridSystem.CheckGridBounds(endGridPosition);

            HandleMove(_beginTouchGridPosition.hitGridPosition, endGridPosition);
        }

        _currentSelectedGridPosition = null;
    }

    private void ResetCurrentGridPosition()
    {
        _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.hitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private async Task HandleMove(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        if (_currentSelectedGridPosition != null) _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.hitGridPosition);

        _allowInput = false;

        var beginGridObject = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var endGridObject = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        if (beginGridObject == null || endGridObject == null || beginGridObject == endGridObject)
        {
            _allowInput = true;
            return;
        }

        if (!beginGridObject.GetMatch3BlockProfile.HasRule("Swap") && !endGridObject.GetMatch3BlockProfile.HasRule("Swap"))
        {
            _allowInput = true;
            return; 
        }

        var swapParameters = new SwapActionParameters
        {   
            from = beginGridObject, 
            to = endGridObject, 
            dataSwapCallback = _gridSystem.SwapGridObjectsData,
            tweenSwapSpeed = levelGridData.VisualSwapSpeed,
            visualSwapCallback = blockVisualManager.MoveVisualWithTweenRoutineAsync,
            GetWorldPositionCallback = _gridSystem.ConvertGridPositionToWorldPosition
        };

        await gridActionProcessor.TryProcessSwapAction(swapParameters);

        var matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        if (!_matchDetector.HasAMatch(matches))
        {
            await gridActionProcessor.TryProcessSwapAction(swapParameters);
            _allowInput = true;
            return;
        }

        // while (true)
        // {
        //     matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        //     if (!_matchDetector.HasAMatch(matches)) break;

        //     yield return DestroyMatches(matches);
        //     yield return CollapseAndFill();
        // }

        // CheckForPossibleMoves();

        _allowInput = true;
    }

    private void CheckForPossibleMoves()
    {
        var grid = _gridSystem.GetGridObjectArray;
        if (_matchDetector.PlayerHasPossibleMoves(grid, _gridSystem.SwapGridObjectsData)) return;

        foreach (var gridObject in grid)
        {
            blockVisualManager.TryDisableVisualOnGridObject(gridObject);
            gridObject.SetMatch3BlockProfile(null);
        }

        ReshuffleGrid();
    }

    private IEnumerator DestroyMatches(HashSet<Match> matches)
    {
        foreach (var match in matches)
        {
            OnMatchDestroyed?.Invoke(match.MatchAttack);
        }

        _gridSystem.DisposeMatchData(matches);
        yield return blockVisualManager.DisableMatchesVisuals(matches);
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
                newMatch3Profile.Init();
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
}
