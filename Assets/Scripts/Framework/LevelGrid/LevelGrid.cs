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
        if (!_gridSystem.IsValidGridPosition(newGridHit.hitGridPosition)) return;

        _beginTouchGridPosition = newGridHit;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        var newGridHit = _gridSystem.ConvertWorldPositionToGridHit(fingerPosition);

        var endTouchGridPosition = newGridHit;

        if (!_gridSystem.IsValidGridPosition(_beginTouchGridPosition.hitGridPosition))
        {
            _currentSelectedGridPosition = null;
            return;
        }

        //Net toegevoegt
        if (endTouchGridPosition.hitGridPosition == _beginTouchGridPosition.hitGridPosition && _currentSelectedGridPosition == null)
        {
            _currentSelectedGridPosition = _beginTouchGridPosition;
            return;
        }

        var isClickMove = _beginTouchGridPosition.hitGridPosition == endTouchGridPosition.hitGridPosition;

        if (isClickMove)
        {
            var startPos = _currentSelectedGridPosition.Value.hitGridPosition;

            var endGridPosition = CalculateClickedEndGridPosition(startPos, endTouchGridPosition.rawX, endTouchGridPosition.rawY);

            endGridPosition = CheckGridBounds(endGridPosition);

            if (IsDiagonalMove(startPos, endGridPosition))
                return;

            OnRequestGridObjectSwap(startPos, endGridPosition);
        }
        else
        {
            var endGridPosition = CalculateSwipeEndGridPosition(_beginTouchGridPosition.hitGridPosition, endTouchGridPosition.rawX, endTouchGridPosition.rawY);

            endGridPosition = CheckGridBounds(endGridPosition);

            OnRequestGridObjectSwap(_beginTouchGridPosition.hitGridPosition, endGridPosition);
        }

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
        int x = Mathf.Clamp(pos.X, 0, levelGridData.GridWidth - 1);
        int y = Mathf.Clamp(pos.Y, 0, levelGridData.GridHeight - 1);
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

        if (distanceX >= 1 && distanceY >= 1) return beginGridPosition;


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

    private GridPosition CalculateSwipeEndGridPosition(GridPosition beginGridPosition, float rawX, float rawY)
    {
        int startX = beginGridPosition.X;
        int startY = beginGridPosition.Y;

        float deltaX = rawX - startX;
        float deltaY = rawY - startY;

        float tolerance = levelGridData.SwipeDirectionTolerance;
        float maxDiagonalDeviation = levelGridData.SwipeMaxDiagonalDeviation; // <-- nieuw

        bool dominantHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);
        bool dominantVertical   = !dominantHorizontal;

        // 2. Richting bepalen met tolerance
        bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY) + tolerance;
        bool isVertical   = Mathf.Abs(deltaY) > Mathf.Abs(deltaX) + tolerance;

        if (!isHorizontal && !isVertical)
        {
            // fallback naar dominante richting
            isHorizontal = dominantHorizontal;
            isVertical   = dominantVertical;
        }

        // 3. Te schuin?
        if (isHorizontal && Mathf.Abs(deltaY) > maxDiagonalDeviation)
            return beginGridPosition;

        if (isVertical && Mathf.Abs(deltaX) > maxDiagonalDeviation)
            return beginGridPosition;

        // 4. Richting uitvoeren
        if (isHorizontal)
            return new GridPosition(startX + (deltaX > 0 ? 1 : -1), startY);

        return new GridPosition(startX, startY + (deltaY > 0 ? 1 : -1));



        // var startX = beginGridPosition.X;
        // var startY = beginGridPosition.Y;

        // float deltaX = rawX - startX;
        // float deltaY = rawY - startY;

        // float tolerance = levelGridData.swapTolerance;

        // bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY) + tolerance;
        // bool isVertical   = Mathf.Abs(deltaY) > Mathf.Abs(deltaX) + tolerance;

        // // Als geen van beide duidelijk is → kies de dominante richting zonder tolerance
        // if (!isHorizontal && !isVertical)
        // {
        //     isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);
        //     isVertical   = !isHorizontal;
        // }

        // if (isHorizontal)
        // {
        //     if (deltaX > 0) return new GridPosition(startX + 1, startY);
        //     else            return new GridPosition(startX - 1, startY);
        // }

        // if (isVertical)
        // {
        //     if (deltaY > 0) return new GridPosition(startX, startY + 1);
        //     else            return new GridPosition(startX, startY - 1);
        // }

        // return beginGridPosition;
    }

    private void OnRequestGridObjectSwap(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);
    }
}
