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
                newGridObjectVisual.Initialize(gridObjectArray[x, y]);  
                OnNewGridObjectCreated?.Invoke(newGridObjectVisual.transform);
            }
        }      
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) =>  new Vector3(gridPosition.X, gridPosition.Y, 0);

    public GridPosition GetWorldToGridPosition(Vector2 worldPosition)
    {   
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_gridRectTransform, worldPosition, null, out var localPos);

        int gridX = Mathf.FloorToInt(localPos.x / _cellWidth);
        int gridY = Mathf.FloorToInt(localPos.y / _cellHeight);

        return new GridPosition(gridX, gridY);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < _width && gridPosition.Y < _height; 
}
