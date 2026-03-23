using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int TileSize = 1;

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y) + Vector3.right  * .2f, Color.white, 1000);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y) =>  new Vector3(x, y, 0) * TileSize;
}
