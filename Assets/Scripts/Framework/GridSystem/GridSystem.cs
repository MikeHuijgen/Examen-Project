using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem
{
    private int _width;
    private int _height;
    private float _cellWidth;
    private float _cellHeight;

    private GridObject[,] _gridObjectArray;

    private Dictionary<GridPosition, GridTileVisual> _gridTileVisuals = new Dictionary<GridPosition, GridTileVisual>();

    public GridSystem(int width, int height, float cellWidth, float cellHeight)
    {
        _width = width;
        _height = height;
        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height;
    public GridObject GetGridObjectByGridPosition(GridPosition gridPosition) => IsValidGridPosition(gridPosition) ? _gridObjectArray[gridPosition.X, gridPosition.Y] : null;

    public GridObject[,] GetGridObjectArray => _gridObjectArray;

    public void GenerateGrid()
    {
        _gridObjectArray = new GridObject[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridPosition = new GridPosition(x, y);
                var newGridObject = new GridObject(newGridPosition);
                _gridObjectArray[x, y] = newGridObject;
            }
        }
    }

    public void CreateGridTileVisuals(GridTileVisual gridTileVisual)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newTileVisual = GameObject.Instantiate(gridTileVisual, new Vector3(x * _cellWidth, y * _cellHeight, 0), quaternion.identity);
                newTileVisual.transform.localScale = new Vector3(_cellWidth, _cellHeight, 0);
                _gridTileVisuals[new GridPosition(x,y)] = newTileVisual;
            }
        }
    }

    public Vector3 ConvertGridPositionToWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.X * _cellWidth + _cellWidth / 2, gridPosition.Y * _cellHeight + _cellHeight / 2, 0);
    }


    public GridHit ConvertScreenPositionToGridHit(Vector2 worldPosition)
    {
        var localPos = CameraHolder.Match3Camera.ScreenToWorldPoint(worldPosition);

        var rawX = localPos.x / _cellWidth;
        var rawY = localPos.y / _cellHeight;

        var gridX = Mathf.FloorToInt(rawX);
        var gridY = Mathf.FloorToInt(rawY);

        return new GridHit(new GridPosition(gridX, gridY), rawX, rawY, localPos);
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

    public GridPosition CalculateClickedEndGridPosition(GridPosition beginGridPosition, float rawX, float rawY, float clickTolerance)
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
            rawX -= clickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX >= 2) return new GridPosition(startX, startY);
            return new(startX + 1, startY);

        }
        else if (rawX < startX)
        {
            rawX += clickTolerance;
            var newGridPositionX = Mathf.FloorToInt(rawX);
            distanceX = newGridPositionX - startX;

            if (distanceX <= -2) return new GridPosition(startX, startY);
            return new(startX - 1, startY);
        }

        if (rawY > startY)
        {
            rawY -= clickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY >= 2) return new GridPosition(startX, startY);
            return new(startX, startY + 1);
        }
        else if (rawY < startY)
        {
            rawY += clickTolerance;
            var newGridPositionY = Mathf.FloorToInt(rawY);
            distanceY = newGridPositionY - startY;

            if (distanceY <= -2) return new GridPosition(startX, startY);
            return new(startX, startY - 1);
        }

        return beginGridPosition;
    }

    public GridPosition CalculateSwipeEndGridPosition(GridHit beginHit, GridHit endHit, float swipeDirectionTolerance, float swipeMaxDiagonalDeviation)
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
            return new GridPosition(beginHit.hitGridPosition.X + dir, beginHit.hitGridPosition.Y);
        }
        else
        {
            var dir = delta.y > 0 ? 1 : -1;
            return new GridPosition(beginHit.hitGridPosition.X, beginHit.hitGridPosition.Y + dir);
        }

    }

    public bool IsDiagonalMove(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var deltaGridX = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var deltaGridY = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        return deltaGridX >= 1 && deltaGridY >= 1;
    }

    public GridPosition CheckGridBounds(GridPosition pos)
    {
        var x = Mathf.Clamp(pos.X, 0, _width - 1);
        var y = Mathf.Clamp(pos.Y, 0, _height - 1);
        return new GridPosition(x, y);
    }

    public void SelectTileByGridPosition(GridPosition targetGridPosition)
    {
       if(!_gridTileVisuals.TryGetValue(targetGridPosition, out var gridTileVisual)) return;
       gridTileVisual.OnTileSelected();
    }

    public void DeselectTileByGridPosition(GridPosition targetGridPosition)
    {
       if(!_gridTileVisuals.TryGetValue(targetGridPosition, out var gridTileVisual)) return;
       gridTileVisual.OnTileDeselected();
    }
}
