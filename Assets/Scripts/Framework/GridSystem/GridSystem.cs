using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem
{
    public static event Action<Transform> OnNewGridObjectCreated;
    public static event Action OnSwappedGridObjects;

    private int _width;
    private int _height;
    private int _cellWidth;
    private int _cellHeight;
    private float _swapTolerance;
    private RectTransform _gridRectTransform;

    private GridObject[,] _gridObjectArray;

    public GridSystem(int width, int height, int cellWidth, int cellHeight, float swapTolerance)
    {
        _width = width;
        _height = height;
        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
        _swapTolerance = swapTolerance;
    }

    public void SetRectTransform(RectTransform rect) => _gridRectTransform = rect;
    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height;
    public GridObject GetGridObjectByGridPosition(GridPosition gridPosition) => IsValidGridPosition(gridPosition) ? _gridObjectArray[gridPosition.X, gridPosition.Y] : null;

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

    public void CreateGridObjectVisualUIs(GridObjectVisualUI gridObjectVisualUI)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridObjectVisualUI = GameObject.Instantiate(gridObjectVisualUI);

                newGridObjectVisualUI.Initialize(_gridObjectArray[x, y], _cellWidth, _cellHeight, GetGridPositionToWorldPosition);

                _gridObjectArray[x, y].SetGridObjectVisualUI(newGridObjectVisualUI);

                OnNewGridObjectCreated?.Invoke(newGridObjectVisualUI.transform);

                var rect = newGridObjectVisualUI.GetComponent<RectTransform>();
                rect.anchoredPosition = GetGridPositionToWorldPosition(new GridPosition(x, y));
            }
        }
    }

    public void CreateGridObjectVisuals(List<GridObjectVisual> gridObjectVisualPrefabs)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var gridObjectVisualUI = _gridObjectArray[x, y].GetGridObjectVisualUI;
                var randomGridVisual = gridObjectVisualPrefabs[UnityEngine.Random.Range(0, gridObjectVisualPrefabs.Count)];
                var newGridObjectVisual = GameObject.Instantiate(randomGridVisual);
                newGridObjectVisual.Initialize(_gridObjectArray[x, y], gridObjectVisualUI.GetRectToWorldTransform);

                _gridObjectArray[x, y].SetGridObjectVisual(newGridObjectVisual);
            }
        }
    }

    public Vector3 GetGridPositionToWorldPosition(GridPosition gridPosition)
    {
        var gridWidthPx = _width * _cellWidth;
        var gridHeightPx = _height * _cellHeight;

        var offsetX = -gridWidthPx / 2f;
        var offsetY = -gridHeightPx / 2f;

        var x = offsetX + gridPosition.X * _cellWidth + _cellWidth * 0.5f;
        var y = offsetY + gridPosition.Y * _cellHeight + _cellHeight * 0.5f;

        return new Vector3(x, y, 0);
    }


    public GridPosition? GetWorldPositionToGridPosition(Vector2 worldPosition, bool useTolerance, GridPosition? gridPosition = null)
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
        

        if (gridPosition != null)
        {
            var startGridPos = gridPosition.Value;
            var gridXValue = startGridPos.X;
            var gridYValue = startGridPos.Y;

            var deltaX = rawX - startGridPos.X;
            var deltaY = rawY - startGridPos.Y;

            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                rawY = startGridPos.Y;
            }
            else
            {
                rawX = startGridPos.X;
            }   

            var dx = Mathf.Abs(gridX - startGridPos.X);
            var dy = Mathf.Abs(gridY - startGridPos.Y);


            if (dx >= 1 && dy >= 1) return null;

            if ((dx >= 1 && dy < 1) || (dx < 1 && dy >= 1))
            {
                if (rawX > startGridPos.X)  
                    gridXValue++;
                else if (rawX < startGridPos.X)
                    gridXValue--;

                if (rawY > startGridPos.Y)
                    gridYValue++;
                else if (rawY < startGridPos.Y)
                    gridYValue--;                
            }

            return new GridPosition(gridXValue, gridYValue);
        }

        return new GridPosition(gridX, gridY);
    }

    public void SwapGridObjects(GridObject gridObjectA, GridObject gridObjectB)
    {
        var gridPositionA = gridObjectA.GetGridPosition;
        var gridPositionB = gridObjectB.GetGridPosition;

        _gridObjectArray[gridPositionA.X, gridPositionA.Y] = gridObjectB;
        _gridObjectArray[gridPositionB.X, gridPositionB.Y] = gridObjectA;

        gridObjectA.SetGridPosition(gridPositionB);
        gridObjectB.SetGridPosition(gridPositionA);

        OnSwappedGridObjects?.Invoke();
    }
}
