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
    [SerializeField] private AttackToMatch3Block[] attackToMatch3Blocks;
    [SerializeField] private Match3BlockPool _match3BlockPool;
    [SerializeField] private Match3BlockProfileContainer match3BlockProfileContainer;
    [SerializeField] private BlockVisualManager blockVisualManager;
    private Dictionary<BaseAttack, Match3BlockVisual> _attackToMatch3BlocksDictionary;
    private GridSystem _gridSystem;
    private MatchDetector _matchDetector;
    private GridHit _beginTouchGridPosition;
    private GridHit? _currentSelectedGridPosition;
    private List<BaseAttack> _attackKeys;
    private List<Tween> _tweens = new List<Tween>();
    private List<(Match3BlockVisual block, BaseAttack attack)> _tiles = new();
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

        FillDictionary();

        _attackKeys = new List<BaseAttack>(_attackToMatch3BlocksDictionary.Keys.ToList());
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }

    private void FillDictionary()
    {
        _attackToMatch3BlocksDictionary = new Dictionary<BaseAttack, Match3BlockVisual>();
        foreach (var attackToMatch3Block in attackToMatch3Blocks)
        {
            if (_attackToMatch3BlocksDictionary.ContainsKey(attackToMatch3Block.attackData)) continue;

            _attackToMatch3BlocksDictionary.Add(attackToMatch3Block.attackData, attackToMatch3Block.Match3Block);
        }
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

        if (!beginGridObject.GetMatch3BlockProfile.HasAction<SwapAction>() || !endGridObject.GetMatch3BlockProfile.HasAction<SwapAction>()) 
        {
            print($"Has no swap action by: {beginGridObject.GetMatch3BlockProfile} and {endGridObject.GetMatch3BlockProfile}");
            _allowInput = true;
            yield break;
        }

        print("Allow swap");

        // _gridSystem.SwapGridObjectsData(beginGridObject, endGridObject);

        // yield return MoveVisuals(beginGridObject, endGridObject);

        // var matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        // if (!HasAMatch(matches))
        // {
        //     _gridSystem.SwapGridObjectsData(beginGridObject, endGridObject);
        //     yield return MoveVisuals(beginGridObject, endGridObject);
        //     _allowInput = true;
        //     yield break;
        // }

        // while (true)
        // {
        //     matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        //     if (!HasAMatch(matches)) break;

        //     yield return DestroyMatches(matches);

        //     yield return CollapseAndFill();
        // }

        // CheckForPossibleMoves();

        _allowInput = true;
    }

    private void CheckForPossibleMoves()
    {
        var grid = _gridSystem.GetGridObjectArray;
        if (PlayerHasPossibleMoves(grid)) return;

        foreach (var gridObject in grid)
        {
            _match3BlockPool.ReturnMatch3Block(gridObject.GetGridMatch3Block);
            gridObject.SetAttackData(null);
            gridObject.SetMatch3Block(null);
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
        var newBeginGridObjectPosition = beginGridObject.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight);
        var newEndGridObjectPosition = endGridObject.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight);

        beginGridObject.GetGridMatch3Block.transform.DOMove(newBeginGridObjectPosition, levelGridData.VisualSwapSpeed).SetEase(Ease.InOutQuad);
        yield return endGridObject.GetGridMatch3Block.transform.DOMove(newEndGridObjectPosition, levelGridData.VisualSwapSpeed).SetEase(Ease.InOutQuad).WaitForCompletion();
    }

    private IEnumerator DestroyMatches(HashSet<Match> matches)
    {
        var grid = _gridSystem.GetGridObjectArray;
        var lastAttack = default(BaseAttack);

        foreach (var match in matches)
        {
            foreach (var position in match.GridPositions)
            {
                var block = grid[position.X, position.Y].GetGridMatch3Block;
                if (block == null) continue;
                _match3BlockPool.ReturnMatch3Block(block);

                grid[position.X, position.Y].SetMatch3Block(null);
                grid[position.X, position.Y].SetAttackData(null);
            }
            
            OnMatchDestroyed?.Invoke(match.AttackData);
        }

        yield return new WaitForSeconds(.15f);
    }

    public void ReshuffleGrid()
    {
        var grid = _gridSystem.GetGridObjectArray;
        for (var x = 0; x < levelGridData.GridWidth; x++)
        {
            for (int y = 0; y < levelGridData.GridHeight; y++)
            {
                var newMatch3Profile = match3BlockProfileContainer.GetRandomProfile();
                if (!blockVisualManager.TryEnableBlockByProfile(newMatch3Profile, new GridPosition(x, y), _gridSystem.ConvertGridPositionToWorldPosition)) continue;
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
                var block = grid[x, y].GetGridMatch3Block;
                if (block == null) continue;

                _tiles.Add((block, grid[x, y].GetAttackData));
            }

            for (var y = 0; y < levelGridData.GridHeight; y++)
            {
                grid[x, y].SetMatch3Block(null);
                grid[x, y].SetAttackData(null);
            }

            foreach (var tile in _tiles)
            {
                var targetGrid = grid[x, writeY];
                var targetPos = new Vector2(
                x * levelGridData.GridCellWidth + levelGridData.GridCellWidth / 2,
                writeY * levelGridData.GridCellHeight + levelGridData.GridCellHeight / 2);


                targetGrid.SetMatch3Block(tile.block);
                targetGrid.SetAttackData(tile.attack);

                var tween = tile.block.transform.DOMove(targetGrid.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight), levelGridData.VisualFallSpeed).SetEase(Ease.OutBounce, .7f);

                _tweens.Add(tween);


                writeY++;
            }

            var spawnCount = levelGridData.GridHeight - writeY;

            for (var i = 0; i < spawnCount; i++)
            {
                var y = writeY + i;

                var attackData = _matchDetector.GetRandomValidAttackData(_attackKeys, grid, x, y);

                if(!_match3BlockPool.GetMatch3BlockByAttackData(attackData, out var newMatch3Block)) continue;

                var targetGrid = grid[x, y];
                var targetPos = new Vector2(
                    x * levelGridData.GridCellWidth + levelGridData.GridCellWidth / 2,
                    y * levelGridData.GridCellHeight + levelGridData.GridCellHeight / 2);

                var spawnY = targetPos.y + (spawnCount - i) + 5f;
                newMatch3Block.transform.position = new Vector3(targetPos.x, spawnY, 0);

                targetGrid.SetMatch3Block(newMatch3Block);
                targetGrid.SetAttackData(attackData);

                var tween = newMatch3Block.transform.DOMove(targetGrid.GetWorldPosition(levelGridData.GridCellWidth, levelGridData.GridCellHeight), levelGridData.VisualFallSpeed).SetEase(Ease.OutBounce, .7f);

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
