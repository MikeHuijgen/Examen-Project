using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem
{
    public static event Action<Transform> OnNewGridObjectCreated;

    private int _width;
    private int _height;
    private int _cellWidth;
    private int _cellHeight;
    private RectTransform _gridRectTransform;

    private GridObject[,] _gridObjectArray;

    public GridSystem(int width, int height, int cellWidth, int cellHeight)
    {
        _width = width;
        _height = height;
        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
    }

    public void SetRectTransform(RectTransform rect) => _gridRectTransform = rect;
    public bool IsValidGridPosition(gridObject gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height;
    public GridObject GetGridObjectByGridPosition(gridObject gridPosition) => IsValidGridPosition(gridPosition) ? _gridObjectArray[gridPosition.X, gridPosition.Y] : null;

    public GridObject[,] GetGridObjectArray => _gridObjectArray;

    public void GenerateGrid()
    {
        _gridObjectArray = new GridObject[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridPosition = new gridObject(x, y);
                var newGridObject = new GridObject(newGridPosition);
                _gridObjectArray[x, y] = newGridObject;
            }
        }
    }

    public void CreateGridObjectVisualUIs(GridObjectVisualUI gridObjectVisualUI)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridObjectVisualUI = GameObject.Instantiate(gridObjectVisualUI);

                newGridObjectVisualUI.Initialize(_gridObjectArray[x, y], _cellWidth, _cellHeight, ConvertGridPositionToWorldPosition);

                _gridObjectArray[x, y].SetGridObjectVisualUI(newGridObjectVisualUI);

                OnNewGridObjectCreated?.Invoke(newGridObjectVisualUI.transform);

                var rect = newGridObjectVisualUI.GetComponent<RectTransform>();
                rect.anchoredPosition = ConvertGridPositionToWorldPosition(new gridObject(x, y));
            }
        }
    }

    public Vector3 ConvertGridPositionToWorldPosition(gridObject gridPosition)
    {
        var gridWidthPx = _width * _cellWidth;
        var gridHeightPx = _height * _cellHeight;

        var offsetX = -gridWidthPx / 2f;
        var offsetY = -gridHeightPx / 2f;

        var x = offsetX + gridPosition.X * _cellWidth + _cellWidth * 0.5f;
        var y = offsetY + gridPosition.Y * _cellHeight + _cellHeight * 0.5f;

        return new Vector3(x, y, 0);
    }


    public GridHit ConvertWorldPositionToGridHit(Vector2 worldPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _gridRectTransform,
            worldPosition,
            CameraHolder.Match3Camera,
            out var localPos
        );

        var gridWidthPx = _width * _cellWidth;
        var gridHeightPx = _height * _cellHeight;

        var offsetX = -gridWidthPx / 2f;
        var offsetY = -gridHeightPx / 2f;

        var rawX = (localPos.x - offsetX) / _cellWidth;
        var rawY = (localPos.y - offsetY) / _cellHeight;

        var gridX = Mathf.FloorToInt(rawX);
        var gridY = Mathf.FloorToInt(rawY);

        return new GridHit(new gridObject(gridX, gridY), rawX, rawY, localPos);
    }

    public void SwapGridObjectsData(GridObject gridObjectA, GridObject gridObjectB)
    {
        var gridPositionA = gridObjectA.GetGridPosition;
        var gridPositionB = gridObjectB.GetGridPosition;

        _gridObjectArray[gridPositionA.X, gridPositionA.Y] = gridObjectB;
        _gridObjectArray[gridPositionB.X, gridPositionB.Y] = gridObjectA;

        gridObjectA.SetGridPosition(gridPositionB);
        gridObjectB.SetGridPosition(gridPositionA);
    }

    public gridObject CalculateClickedEndGridPosition(gridObject beginGridPosition, float rawX, float rawY, float clickTolerance)
    {
        var startX = beginGridPosition.X;
        var startY = beginGridPosition.Y;

        var deltaX = rawX - startX;
        var deltaY = rawY - startY;

        var distanceX = Mathf.FloorToInt(rawX) - startX;
        var distanceY = Mathf.FloorToInt(rawY) - startY;

        if (Mathf.Abs(deltaX) >= 3f || Mathf.Abs(deltaY) >= 3f) return beginGridPosition;

        if (Mathf.Abs(distanceX) >= 1 && Mathf.Abs(distanceY) >= 1) return beginGridPosition;
        if (Mathf.Abs(distanceX) == 0 && Mathf.Abs(distanceY) == 0) return beginGridPosition;


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
            rawX -= clickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX >= 2) return new gridObject(startX, startY);
            return new(startX + 1, startY);

        }
        else if (rawX < startX)
        {
            rawX += clickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX <= -2) return new gridObject(startX, startY);
            return new(startX - 1, startY);
        }

        if (rawY > startY)
        {
            rawY -= clickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY >= 2) return new gridObject(startX, startY);
            return new(startX, startY + 1);
        }
        else if (rawY < startY)
        {
            rawY += clickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY <= -2) return new gridObject(startX, startY);
            return new(startX, startY - 1);
        }

        return beginGridPosition;
    }

    public gridObject CalculateSwipeEndGridPosition(GridHit beginHit, GridHit endHit, float swipeDirectionTolerance, float swipeMaxDiagonalDeviation)
    {
        var delta = endHit.localPos - beginHit.localPos;

        var absX = Mathf.Abs(delta.x);
        var absY = Mathf.Abs(delta.y);

        var tolerance = swipeDirectionTolerance;
        var maxDiagonalTolerance = swipeMaxDiagonalDeviation;

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

    public bool IsDiagonalMove(gridObject beginGridPosition, gridObject endGridPosition)
    {
        var deltaGridX = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var deltaGridY = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        return deltaGridX >= 1 && deltaGridY >= 1;
    }

    public gridObject CheckGridBounds(gridObject pos)
    {
        var x = Mathf.Clamp(pos.X, 0, _width - 1);
        var y = Mathf.Clamp(pos.Y, 0, _height - 1);
        return new gridObject(x, y);
    }
}
