using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private List<GridObjectVisual> gridObjectVisuals = new List<GridObjectVisual>();
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
            levelGridData.swapTolerance);
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
        if (!_gridSystem.IsValidGridPosition(newGridHit.hitGridPosition)) return;

        _beginTouchGridPosition = newGridHit;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        var newGridHit = _gridSystem.ConvertWorldPositionToGridHit(fingerPosition);
        if (!_gridSystem.IsValidGridPosition(newGridHit.hitGridPosition)) { _currentSelectedGridPosition = null; return; }

        var endTouchGridPosition = newGridHit;

        if (_beginTouchGridPosition.hitGridPosition == endTouchGridPosition.hitGridPosition && _currentSelectedGridPosition == null)
        {
            _currentSelectedGridPosition = newGridHit;
            return;
        }

        var isClickMove = _beginTouchGridPosition.hitGridPosition == endTouchGridPosition.hitGridPosition;

        if (isClickMove)
        {
            if (endTouchGridPosition.hitGridPosition == _currentSelectedGridPosition.Value.hitGridPosition) return;

            if (IsDiagonalMove(_currentSelectedGridPosition.Value.hitGridPosition, endTouchGridPosition.hitGridPosition)) return;

            var endGridPosition = CalculateClickedEndGridPosition(_currentSelectedGridPosition.Value.hitGridPosition, endTouchGridPosition.rawX, endTouchGridPosition.rawY);

            OnRequestGridObjectSwap(_currentSelectedGridPosition.Value.hitGridPosition, endGridPosition);
        }
        else
        {
            if (IsDiagonalMove(_beginTouchGridPosition.hitGridPosition, endTouchGridPosition.hitGridPosition)) return;

            var endGridPosition = CalculateSwipeEndGridPosition(_beginTouchGridPosition.hitGridPosition, endTouchGridPosition.rawX, endTouchGridPosition.rawY);

            OnRequestGridObjectSwap(_beginTouchGridPosition.hitGridPosition, endGridPosition);
        }

        _currentSelectedGridPosition = null;
    }

    private bool IsDiagonalMove(GridPosition beginTouchPosition, GridPosition endTouchPosition)
    {
        var directionX = Mathf.Abs(endTouchPosition.X - beginTouchPosition.X);
        var directionY = Mathf.Abs(endTouchPosition.Y - beginTouchPosition.Y);

        if (directionX >= 1 && directionY >= 1) return true;

        return false;
    }

    private GridPosition CalculateClickedEndGridPosition(GridPosition beginGridPosition, float rawX, float rawY)
    {
        var startX = beginGridPosition.X;
        var startY = beginGridPosition.Y;

        var deltaX = rawX - startX;
        var deltaY = rawY - startY;

        var distanceX = Mathf.FloorToInt(rawX) - startX;
        var distanceY = Mathf.FloorToInt(rawY) - startY;

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
            rawX -= levelGridData.swapTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX >= 2) return new GridPosition(startX, startY);
            return new(startX + 1, startY);

        }
        else if (rawX < startX)
        {
            rawX += levelGridData.swapTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX <= -2) return new GridPosition(startX, startY);
            return new(startX - 1, startY);
        }

        if (rawY > startY)
        {
            rawY -= levelGridData.swapTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY >= 2) return new GridPosition(startX, startY);
            return new(startX, startY + 1);
        }
        else if (rawY < startY)
        {
            rawY += levelGridData.swapTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY <= -2) return new GridPosition(startX, startY);
            return new(startX, startY - 1);
        }


        return beginGridPosition;
    }

    private GridPosition CalculateSwipeEndGridPosition(GridPosition beginGridPosition, float rawX, float rawY)
    {
        var startX = beginGridPosition.X;
        var startY = beginGridPosition.Y;

        var deltaX = rawX - startX;
        var deltaY = rawY - startY;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            rawY = startY;
        }
        else
        {
            rawX = startX;
        }

        if (rawX > startX)
            return new GridPosition(startX + 1, startY);
        else if (rawX < startX)
            return new GridPosition(startX - 1, startY);

        if (rawY > startY)
            return new GridPosition(startX, startY + 1);
        else if (rawY < startY)
            return new GridPosition(startX, startY - 1);

        return beginGridPosition;
    }

    private void OnRequestGridObjectSwap(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var dx = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var dy = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        if (dx + dy != 1) return;

        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);
    }
}
