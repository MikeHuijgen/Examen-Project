using System;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem
{
    public static event Action<Transform> OnNewGridObjectCreated;
    private int _width;
    private int _height;
    private int _cellWidth;
    private int _cellHeight;
    private RectTransform _gridRectTransform;

    private GridObject [,] gridObjectArray;

    public GridSystem(int width, int height, int cellWidth, int cellHeight, RectTransform gridRectTransform)
    {
        _width = width;
        _height = height;
        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
        _gridRectTransform = gridRectTransform;
    }

    public void GenerateGrid()
    {
        gridObjectArray = new GridObject[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var newGridPosition = new GridPosition(x, y);
                var newGridObject = new GridObject(newGridPosition);
                gridObjectArray[x, y] = newGridObject;
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

                newGridObjectVisual.Initialize(gridObjectArray[x, y], _cellWidth, _cellHeight);

                OnNewGridObjectCreated?.Invoke(newGridObjectVisual.transform);

                RectTransform rect = newGridObjectVisual.GetComponent<RectTransform>();
                rect.anchoredPosition = GetWorldPosition(new GridPosition(x, y));
            }
        }      
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        float gridWidthPx = _width * _cellWidth;
        float gridHeightPx = _height * _cellHeight;

        float offsetX = -gridWidthPx / 2f;
        float offsetY = -gridHeightPx / 2f;

        float x = offsetX + gridPosition.X * _cellWidth + _cellWidth * 0.5f;
        float y = offsetY + gridPosition.Y * _cellHeight + _cellHeight * 0.5f;

        return new Vector3(x, y, 0);
    }


    public GridPosition GetWorldToGridPosition(Vector2 worldPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _gridRectTransform,
            worldPosition,
            null,
            out var localPos
        );

        float gridWidthPx = _width * _cellWidth;
        float gridHeightPx = _height * _cellHeight;

        float offsetX = -gridWidthPx / 2f;
        float offsetY = -gridHeightPx / 2f;

        float x = (localPos.x - offsetX) / _cellWidth;
        float y = (localPos.y - offsetY) / _cellHeight;

        return new GridPosition(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }


    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height; 
}
