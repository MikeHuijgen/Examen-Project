using System;
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

    private GridObject [,] _gridObjectArray;

    public GridSystem(int width, int height, int cellWidth, int cellHeight)
    {
        _width = width;
        _height = height;
        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
    }

    public void SetRectTransform(RectTransform rect) => _gridRectTransform = rect;

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

    public void CreateDebugObjectVisuals(GridObjectDebugVisual gridObjectDebugVisualPrefab)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridObjectVisual = GameObject.Instantiate(gridObjectDebugVisualPrefab);

                newGridObjectVisual.Initialize(_gridObjectArray[x, y], _cellWidth, _cellHeight, GetGridPositionToWorldPosition);

                _gridObjectArray[x,y].SetVisual(newGridObjectVisual);

                OnNewGridObjectCreated?.Invoke(newGridObjectVisual.transform);

                var rect = newGridObjectVisual.GetComponent<RectTransform>();
                rect.anchoredPosition = GetGridPositionToWorldPosition(new GridPosition(x, y));
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


    public GridPosition GetWorldPositionToGridPosition(Vector2 worldPosition)
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

        var x = (localPos.x - offsetX) / _cellWidth;
        var y = (localPos.y - offsetY) / _cellHeight;

        return new GridPosition(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    public void Test(GameObject cube)
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var gridObjectVisual = _gridObjectArray[x,y].GridObjectDebug;
                var position = gridObjectVisual.GetRectToWorldTransform;
                GameObject.Instantiate(cube, position, cube.transform.rotation);
            }
        }   
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height; 

    public GridObject GetGridObjectByGridPosition(GridPosition gridPosition) => IsValidGridPosition(gridPosition) ? _gridObjectArray[gridPosition.X, gridPosition.Y] : null;

    public void SwapGridObjects(GridObject gridObjectA, GridObject gridObjectB)
    {
        var gridPositionA = gridObjectA.GetGridPosition;
        var gridPositionB = gridObjectB.GetGridPosition;

        _gridObjectArray[gridPositionA.X, gridPositionA.Y] = gridObjectB;
        _gridObjectArray[gridPositionB.X, gridPositionB.Y] = gridObjectA;

        gridObjectA.SetGridPosition(gridPositionB);
        gridObjectB.SetGridPosition(gridPositionA);
    }
}
