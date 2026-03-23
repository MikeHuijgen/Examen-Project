using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance;
    public event Action<Vector2> OnNewGeneratedGrid;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int gridCellWidth = 100;
    [SerializeField] private int gridCellHeight = 100;
    [SerializeField] private Transform gridObjectVisualHolder;
    [SerializeField] private RectTransform gridRectTransform;

    private GridObject [,] gridObjectArray;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);

        Instance = this;
        GenerateGrid();
    }

    private void Start() => OnNewGeneratedGrid?.Invoke(new Vector2(gridCellWidth, gridCellHeight));

    private void GenerateGrid()
    {
        gridObjectArray = new GridObject[width, height];

        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newGridPosition = new GridPosition(x, y);
                var newGridObject = new GridObject(newGridPosition);
                gridObjectArray[x, y] = newGridObject;
            }
        }
    }

    public void CreateDebugTileVisuals(GridObjectDebugVisual gridObjectDebugVisualPrefab)
    {
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newGridTileVisual = Instantiate(gridObjectDebugVisualPrefab, gridObjectVisualHolder);
                newGridTileVisual.Initialize(gridObjectArray[x, y]);  
            }
        }      
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) =>  new Vector3(gridPosition.X, gridPosition.Y, 0);

    public GridPosition GetWorldToGridPosition(Vector2 worldPosition)
    {   
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRectTransform, worldPosition, null, out var localPos);

        int gridX = Mathf.FloorToInt(localPos.x / gridCellWidth);
        int gridY = Mathf.FloorToInt(localPos.y / gridCellHeight);

        return new GridPosition(gridX, gridY);
    }

    private bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.X >= 0 && gridPosition.Y >= 0 && gridPosition.X < width && gridPosition.Y < height;
    }
}
