using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEditor.Rendering;
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
            var endGridPosition = CalculateClickedEndGridPosition(_currentSelectedGridPosition.Value.hitGridPosition, endTouchGridPosition.rawX, endTouchGridPosition.rawY);

            endGridPosition = CheckGridBounds(endGridPosition);

            if (endGridPosition == _currentSelectedGridPosition.Value.hitGridPosition)
            {
                ResetCurrentGridPosition();
                return;
            }

            if (IsDiagonalMove(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition))
            {
                ResetCurrentGridPosition();
                return;
            }


            StartCoroutine(HandleMove(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition));
        }
        else
        {
            var endGridPosition = CalculateSwipeEndGridPosition(_beginTouchGridPosition, endTouchGridPosition);

            endGridPosition = CheckGridBounds(endGridPosition);

            StartCoroutine(HandleMove(_beginTouchGridPosition.hitGridPosition, endGridPosition));
        }

        _currentSelectedGridPosition = null;
    }

    private void ResetCurrentGridPosition()
    {
        OnTileDeselected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private bool IsDiagonalMove(gridObject beginGridPosition, gridObject endGridPosition)
    {
        var deltaGridX = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var deltaGridY = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        return deltaGridX >= 1 && deltaGridY >= 1;
    }

    private gridObject CheckGridBounds(gridObject pos)
    {
        var x = Mathf.Clamp(pos.X, 0, levelGridData.GridWidth - 1);
        var y = Mathf.Clamp(pos.Y, 0, levelGridData.GridHeight - 1);
        return new gridObject(x, y);
    }


    private gridObject CalculateClickedEndGridPosition(gridObject beginGridPosition, float rawX, float rawY)
    {
        var startX = beginGridPosition.X;
        var startY = beginGridPosition.Y;

        var deltaX = rawX - startX;
        var deltaY = rawY - startY;

        var distanceX = Mathf.FloorToInt(rawX) - startX;
        var distanceY = Mathf.FloorToInt(rawY) - startY;

        if (Mathf.Abs(deltaX) >= 3f || Mathf.Abs(deltaY) >= 3f) return beginGridPosition;

        if (Mathf.Abs(distanceX) >= 1 && Mathf.Abs(distanceY) >= 1) return beginGridPosition;


        if (distanceX == 1) return new gridObject(startX + 1, startY);
        if (distanceX == -1) return new gridObject(startX - 1, startY);
        if (distanceY == 1) return new gridObject(startX, startY + 1);
        if (distanceY == -1) return new gridObject(startX, startY - 1);

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            rawY = startY;
        }
        else
        {
            rawX = startX;
        }


        if (rawX > startX)
        {
            rawX -= levelGridData.ClickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX >= 2) return new gridObject(startX, startY);
            return new(startX + 1, startY);

        }
        else if (rawX < startX)
        {
            rawX += levelGridData.ClickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX <= -2) return new gridObject(startX, startY);
            return new(startX - 1, startY);
        }

        if (rawY > startY)
        {
            rawY -= levelGridData.ClickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY >= 2) return new gridObject(startX, startY);
            return new(startX, startY + 1);
        }
        else if (rawY < startY)
        {
            rawY += levelGridData.ClickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY <= -2) return new gridObject(startX, startY);
            return new(startX, startY - 1);
        }

        return beginGridPosition;
    }

    private gridObject CalculateSwipeEndGridPosition(GridHit beginHit, GridHit endHit)
    {
        var delta = endHit.localPos - beginHit.localPos;

        var absX = Mathf.Abs(delta.x);
        var absY = Mathf.Abs(delta.y);

        var tolerance = levelGridData.SwipeDirectionTolerance;
        var maxDiagonalTolerance = levelGridData.SwipeMaxDiagonalDeviation;

        var ratio = absX > absY ? absY / absX : absX / absY;

        if (ratio > maxDiagonalTolerance)
            return beginHit.hitGridPosition;

        if (ratio > tolerance)
            return beginHit.hitGridPosition;

        var horizontal = absX > absY;

        if (horizontal)
        {
            var dir = delta.x > 0 ? 1 : -1;
            return new gridObject(beginHit.hitGridPosition.X + dir, beginHit.hitGridPosition.Y);
        }
        else
        {
            var dir = delta.y > 0 ? 1 : -1;
            return new gridObject(beginHit.hitGridPosition.X, beginHit.hitGridPosition.Y + dir);
        }

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

        while (HasAMatch(matches))
        {
            yield return DestroyMatches(matches);

            yield return CollapseColumns();

            yield return FillBoard();
            matches = _matchDetector.CheckForAllMatches(_gridSystem.GetGridObjectArray, levelGridData.GridWidth, levelGridData.GridHeight);            
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
        foreach (var position in matches)
        {
            var grid = _gridSystem.GetGridObjectArray;
            Destroy(grid[position.X, position.Y].GetGridMatch3Block.gameObject);
            grid[position.X, position.Y].SetMatch3Block(null);
            grid[position.X, position.Y].SetAttackData(null);
        }

        matches.Clear();
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

    private IEnumerator CollapseColumns()
    {
        var grid = _gridSystem.GetGridObjectArray;
        var tweens = new List<Tween>();

        for (int x = 0; x < levelGridData.GridWidth; x++)
        {
            int writeY = 0;

            // 1. Verzamel alle tiles
            List<(GridObject gridObj, Match3Block block, FakeAttack attack)> tiles = new();

            for (int y = 0; y < levelGridData.GridHeight; y++)
            {
                var block = grid[x, y].GetGridMatch3Block;
                if (block != null)
                    tiles.Add((grid[x, y], block, grid[x, y].GetAttackData));
            }

            // 2. Laat ze vallen
            foreach (var tile in tiles)
            {
                var targetGridObj = grid[x, writeY];

                if (tile.gridObj != targetGridObj)
                {
                    var targetPos = targetGridObj.GetGridObjectVisualUI.GetRectToWorldTransform();
                    var startPos = tile.block.transform.position;

                    // 2a. Start tween vanaf huidige positie naar target
                    var tween = tile.block.transform
                        .DOMove(targetPos, levelGridData.VisualFallSpeed)
                        .SetEase(Ease.OutQuint)
                        .SetDelay(0.05f * writeY);

                    tweens.Add(tween);

                    // 2b. Update grid pas NA de tween
                    tween.OnComplete(() =>
                    {
                        tile.block.Initialize(targetGridObj.GetGridObjectVisualUI.GetRectToWorldTransform);
                        targetGridObj.SetMatch3Block(tile.block);
                        targetGridObj.SetAttackData(tile.attack);

                        tile.gridObj.SetMatch3Block(null);
                        tile.gridObj.SetAttackData(null);
                    });
                }

                writeY++;
            }

            // 3. Lege plekken bovenaan
            for (int y = writeY; y < levelGridData.GridHeight; y++)
            {
                grid[x, y].SetMatch3Block(null);
                grid[x, y].SetAttackData(null);
            }
        }

        // 4. Wacht op ALLE tweens
        if (tweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var t in tweens) seq.Join(t);
            yield return seq.WaitForCompletion();
        }
    }

    private IEnumerator FillBoard()
    {
        var grid = _gridSystem.GetGridObjectArray;
        var tweens = new List<Tween>();

        for (int x = 0; x < levelGridData.GridWidth; x++)
        {
            for (int y = 0; y < levelGridData.GridHeight; y++)
            {
                if (grid[x, y].GetGridMatch3Block != null) continue;

                var attackData = _matchDetector.GetRandomValidAttackData(_attackToMatch3BlocksDictionary.Keys.ToList(),grid,x,y);

                var prefab = _attackToMatch3BlocksDictionary[attackData];

                var newBlock = Instantiate(prefab);

                newBlock.Initialize(grid[x, y].GetGridObjectVisualUI.GetRectToWorldTransform);

                float spawnOffset = 5f;

                float spawnY = grid[x, levelGridData.GridHeight - 1].GetGridObjectVisualUI.GetRectToWorldTransform().y + spawnOffset;

                var targetPos = newBlock.GetRectPosition();

                var startPos = new Vector3(targetPos.x, spawnY, targetPos.z);
                newBlock.transform.position = startPos;

                grid[x, y].SetMatch3Block(newBlock);
                grid[x, y].SetAttackData(attackData);

                yield return newBlock.transform.DOMove(targetPos, levelGridData.VisualFallSpeed).SetEase(Ease.OutCubic).WaitForCompletion();
            }
        }
    }

}
