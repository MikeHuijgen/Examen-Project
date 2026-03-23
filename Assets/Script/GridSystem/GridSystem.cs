using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Transform gridTileVisualHolder;
    [SerializeField] private GridTileVisual gridTileVisualPrefab;

    private GridTileData [,] gridTilesArray;

    private void Awake()
    {
        GenerateGrid();
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
}
