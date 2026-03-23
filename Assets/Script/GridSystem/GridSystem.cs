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

    private void Start()
    {
        OnNewGeneratedGrid?.Invoke(new Vector2(gridCellWidth, gridCellHeight));
    }

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

    private Vector3 GetWorldPosition(int x, int y) =>  new Vector3(x, y, 0);

    public GridPosition GetWorldToGridPosition(Vector2 worldPosition)
    {   
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRectTransform, worldPosition, null, out var localPos);

        int gridX = Mathf.FloorToInt(localPos.x / gridCellWidth);
        int gridY = Mathf.FloorToInt(localPos.y / gridCellHeight);

        return new GridPosition(gridX, gridY);
    }
}
