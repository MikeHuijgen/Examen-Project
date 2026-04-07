using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static event Action<gridObject?> OnTileSelected;
    public static event Action<gridObject?> OnTileDeselected;

    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private AttackToMatch3Block[] attackToMatch3Blocks;

    private Dictionary<FakeAttack, Match3Block> _attackToMatch3BlocksDictionary;
    private GridSystem _gridSystem;
    private MatchDetector _matchDetector;
    private GridHit _beginTouchGridPosition;
    private GridHit? _currentSelectedGridPosition;

    private bool _allowInput = true;


    private void Awake()
    {
        _gridSystem = new GridSystem(
            levelGridData.GridWidth,
            levelGridData.GridHeight,
            levelGridData.GridCellWidth,
            levelGridData.GridCellHeight);

        _matchDetector = new MatchDetector();
        GridObjectUIRoot.OnGridRectReady += rect => _gridSystem.SetRectTransform(rect);
        FillDictionary();
    }

    private void FillDictionary()
    {
        _attackToMatch3BlocksDictionary = new Dictionary<FakeAttack, Match3Block>();
        foreach (var attackToMatch3Block in attackToMatch3Blocks)
        {
            if (_attackToMatch3BlocksDictionary.ContainsKey(attackToMatch3Block.FakeAttack)) continue;

            _attackToMatch3BlocksDictionary.Add(attackToMatch3Block.FakeAttack, attackToMatch3Block.Match3Block);
        }
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridObjectVisualUIs(levelGridData.GridObjectDebugVisual);
        ReshuffleGrid();

        CharacterInput.Instance.OnNewFingerDownInput += OnNewFingerDownInput;
        CharacterInput.Instance.OnNewFingerUpInput += OnNewFingerUpInput;
    }

    private void OnNewFingerDownInput(Vector2 fingerPosition)
    {
        var newGridHit = _gridSystem.ConvertWorldPositionToGridHit(fingerPosition);

        _beginTouchGridPosition = newGridHit;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        if (!_allowInput) return;
        var newGridHit = _gridSystem.ConvertWorldPositionToGridHit(fingerPosition);

        var endTouchGridPosition = newGridHit;

        if (endTouchGridPosition.hitGridPosition == _beginTouchGridPosition.hitGridPosition && _currentSelectedGridPosition == null)
        {
            _currentSelectedGridPosition = _beginTouchGridPosition;
            OnTileSelected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);
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
        OnTileDeselected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private IEnumerator HandleMove(gridObject beginGridPosition, gridObject endGridPosition)
    {
        if (_currentSelectedGridPosition != null) OnTileDeselected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);

        _allowInput = false;

        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjectsData(gridObjectA, gridObjectB);

        yield return MoveVisuals(gridObjectA, gridObjectB);

        var matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

        if (!HasAMatch(matches))
        {
            _gridSystem.SwapGridObjectsData(gridObjectA, gridObjectB);
            yield return MoveVisuals(gridObjectA, gridObjectB);
            _allowInput = true;
            yield break;
        }

        while (true)
        {
            matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);

            if (!HasAMatch(matches))break;

            yield return DestroyMatches(matches);

            yield return CollapseAndFill();
        }

        if (!PlayerHasPossibleMoves(_gridSystem.GetGridObjectArray))
        {
            
        }

        _allowInput = true;
    }

    private bool HasAMatch(HashSet<gridObject> matchList)
    {
        if (matchList.Count == 0) return false;

        return true;
    }

    private IEnumerator MoveVisuals(GridObject gridObjectA, GridObject gridObjectB)
    {
        gridObjectA.GetGridMatch3Block.transform.DOMove(gridObjectA.GetGridMatch3Block.GetRectPosition(), levelGridData.VisualSwapSpeed).SetEase(Ease.InQuad);
        yield return gridObjectB.GetGridMatch3Block.transform.DOMove(gridObjectB.GetGridMatch3Block.GetRectPosition(), levelGridData.VisualSwapSpeed).SetEase(Ease.InQuad).WaitForCompletion();
    }

    private IEnumerator DestroyMatches(HashSet<gridObject> matches)
    {
        var grid = _gridSystem.GetGridObjectArray;

        foreach (var position in matches)
        {
            var block = grid[position.X, position.Y].GetGridMatch3Block;

            if (block == null) continue;

            Destroy(block.gameObject);

            grid[position.X, position.Y].SetMatch3Block(null);
            grid[position.X, position.Y].SetAttackData(null);
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
                var gridObjectVisualUI = grid[x, y].GetGridObjectVisualUI;
                var attackData = _matchDetector.GetRandomValidAttackData(_attackToMatch3BlocksDictionary.Keys.ToList(), grid, x, y);
                if (!_attackToMatch3BlocksDictionary.TryGetValue(attackData, out var match3Block)) continue;
                var newMatch3Block = Instantiate(match3Block, gridObjectVisualUI.GetRectToWorldTransform(), quaternion.identity);
                newMatch3Block.Initialize(gridObjectVisualUI.GetRectToWorldTransform);
                grid[x, y].SetMatch3Block(newMatch3Block);
                grid[x, y].SetAttackData(attackData);
            }
        }
    }

    private IEnumerator CollapseAndFill()
    {
        var grid = _gridSystem.GetGridObjectArray;
        var tweens = new List<Tween>();

        for (int x = 0; x < levelGridData.GridWidth; x++)
        {
            var writeY = 0;

            List<(Match3Block block, FakeAttack attack)> tiles = new();

            for (var y = 0; y < levelGridData.GridHeight; y++)
            {
                var block = grid[x, y].GetGridMatch3Block;
                if (block == null) continue;

                tiles.Add((block, grid[x, y].GetAttackData));
            }

            for (var y = 0; y < levelGridData.GridHeight; y++)
            {
                grid[x, y].SetMatch3Block(null);
                grid[x, y].SetAttackData(null);
            }

            foreach (var tile in tiles)
            {
                var targetGrid = grid[x, writeY];
                var targetPos = targetGrid.GetGridObjectVisualUI.GetRectToWorldTransform();

                targetGrid.SetMatch3Block(tile.block);
                targetGrid.SetAttackData(tile.attack);

                var tween = tile.block.transform.DOMove(targetPos, levelGridData.VisualFallSpeed).SetEase(Ease.OutQuint);

                tweens.Add(tween);

                tween.OnComplete(() =>
                {
                    tile.block.Initialize(targetGrid.GetGridObjectVisualUI.GetRectToWorldTransform);
                });

                writeY++;
            }

            var spawnCount = levelGridData.GridHeight - writeY;

            for (var i = 0; i < spawnCount; i++)
            {
                var y = writeY + i;

                var attackData = _matchDetector.GetRandomValidAttackData(_attackToMatch3BlocksDictionary.Keys.ToList(), grid, x, y);

                var prefab = _attackToMatch3BlocksDictionary[attackData];
                var newBlock = Instantiate(prefab);

                var targetGrid = grid[x, y];
                var targetPos = targetGrid.GetGridObjectVisualUI.GetRectToWorldTransform();

                var spawnY = targetPos.y + (spawnCount - i) + 5f;
                newBlock.transform.position = new Vector3(targetPos.x, spawnY, targetPos.z);

                targetGrid.SetMatch3Block(newBlock);
                targetGrid.SetAttackData(attackData);

                var tween = newBlock.transform.DOMove(targetPos, levelGridData.VisualFallSpeed).SetEase(Ease.OutCubic);

                tweens.Add(tween);

                tween.OnComplete(() =>
                {
                    newBlock.Initialize(targetGrid.GetGridObjectVisualUI.GetRectToWorldTransform);
                });
            }
        }

        if (tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in tweens) seq.Join(t);

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
                foreach (var dir in new Vector2Int[] { Vector2Int.right, Vector2Int.up })
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
