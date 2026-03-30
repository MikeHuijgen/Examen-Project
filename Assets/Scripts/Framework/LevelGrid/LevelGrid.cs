using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static event Action<GridPosition?> OnTileSelected;
    public static event Action<GridPosition?> OnTileDeselected;

    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private List<Match3Block> gridObjectVisuals = new List<Match3Block>();
    private GridSystem _gridSystem;
    private GridHit _beginTouchGridPosition;
    private GridHit? _currentSelectedGridPosition;


    private void Awake()
    {
        _gridSystem = new GridSystem(
            levelGridData.GridWidth,
            levelGridData.GridHeight,
            levelGridData.GridCellWidth,
            levelGridData.GridCellHeight,
            levelGridData.SwipeDirectionTolerance);
        GridObjectUIRoot.OnGridRectReady += rect => _gridSystem.SetRectTransform(rect);
    }

    void OnDisable()
    {
        GridObjectUIRoot.OnGridRectReady -= rect => _gridSystem.SetRectTransform(rect);
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridObjectVisualUIs(levelGridData.GridObjectDebugVisual);

        _gridSystem.CreateGridObjectVisuals(gridObjectVisuals);

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


            StartCoroutine(OnRequestGridObjectSwap(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition));
        }
        else
        {
            var endGridPosition = CalculateSwipeEndGridPosition(_beginTouchGridPosition, endTouchGridPosition);

            endGridPosition = CheckGridBounds(endGridPosition);

            StartCoroutine(OnRequestGridObjectSwap(_beginTouchGridPosition.hitGridPosition, endGridPosition));
        }

        _currentSelectedGridPosition = null;
    }

    private void ResetCurrentGridPosition()
    {
        OnTileDeselected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private bool IsDiagonalMove(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var deltaGridX = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var deltaGridY = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        return deltaGridX >= 1 && deltaGridY >= 1;
    }

    private GridPosition CheckGridBounds(GridPosition pos)
    {
        var x = Mathf.Clamp(pos.X, 0, levelGridData.GridWidth - 1);
        var y = Mathf.Clamp(pos.Y, 0, levelGridData.GridHeight - 1);
        return new GridPosition(x, y);
    }


    private GridPosition CalculateClickedEndGridPosition(GridPosition beginGridPosition, float rawX, float rawY)
    {
        var startX = beginGridPosition.X;
        var startY = beginGridPosition.Y;

        var deltaX = rawX - startX;
        var deltaY = rawY - startY;

        var distanceX = Mathf.FloorToInt(rawX) - startX;
        var distanceY = Mathf.FloorToInt(rawY) - startY;

        if (Mathf.Abs(deltaX) >= 3f || Mathf.Abs(deltaY) >= 3f) return beginGridPosition;

        if (Mathf.Abs(distanceX) >= 1 && Mathf.Abs(distanceY) >= 1) return beginGridPosition;


        if (distanceX == 1) return new GridPosition(startX + 1, startY);
        if (distanceX == -1) return new GridPosition(startX - 1, startY);
        if (distanceY == 1) return new GridPosition(startX, startY + 1);
        if (distanceY == -1) return new GridPosition(startX, startY - 1);

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

            if (distanceX >= 2) return new GridPosition(startX, startY);
            return new(startX + 1, startY);

        }
        else if (rawX < startX)
        {
            rawX += levelGridData.ClickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX <= -2) return new GridPosition(startX, startY);
            return new(startX - 1, startY);
        }

        if (rawY > startY)
        {
            rawY -= levelGridData.ClickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY >= 2) return new GridPosition(startX, startY);
            return new(startX, startY + 1);
        }
        else if (rawY < startY)
        {
            rawY += levelGridData.ClickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY <= -2) return new GridPosition(startX, startY);
            return new(startX, startY - 1);
        }

        return beginGridPosition;
    }

    private GridPosition CalculateSwipeEndGridPosition(GridHit beginHit, GridHit endHit)
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
            return new GridPosition(beginHit.hitGridPosition.X + dir, beginHit.hitGridPosition.Y);
        }
        else
        {
            var dir = delta.y > 0 ? 1 : -1;
            return new GridPosition(beginHit.hitGridPosition.X, beginHit.hitGridPosition.Y + dir);
        }

    }


    private IEnumerator OnRequestGridObjectSwap(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        if (_currentSelectedGridPosition != null) OnTileDeselected?.Invoke(_currentSelectedGridPosition.Value.hitGridPosition);
        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);

        gridObjectA.GetGridObjectVisual.transform.DOMove(gridObjectA.GetGridObjectVisual.GetRectPosition(), 0.15f).SetEase(Ease.InQuad);
        yield return gridObjectB.GetGridObjectVisual.transform.DOMove(gridObjectB.GetGridObjectVisual.GetRectPosition(), 0.15f).SetEase(Ease.InQuad).WaitForCompletion();
    }
}
