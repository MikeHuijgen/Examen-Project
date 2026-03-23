using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance;
    public event Action<Vector2> OnNewGeneratedGrid;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int gridCellWidth = 100;
    [SerializeField] private int gridCellHeight = 100;
    [SerializeField] private Transform gridTileVisualHolder;
    [SerializeField] private GridTileVisual gridTileVisualPrefab;
    [SerializeField] private RectTransform gridRectTransform;

    private GridTileData [,] gridTilesArray;

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
        gridTilesArray = new GridTileData[width, height];

        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newGridPosition = new GridPosition(x, y);
                var gridTileData = new GridTileData(newGridPosition);
                gridTilesArray[x, y] = gridTileData;
                var newGridTileVisual = Instantiate(gridTileVisualPrefab, gridTileVisualHolder);
                newGridTileVisual.Initialize(gridTileData);
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
