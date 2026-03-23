using System;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectHolder : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private void OnEnable()
    {
        GridSystem.Instance.OnNewGeneratedGrid += OnNewGeneratedGrid;
    }

    void OnDisable()
    {
        GridSystem.Instance.OnNewGeneratedGrid -= OnNewGeneratedGrid;
    }

    private void OnNewGeneratedGrid(Vector2 gridCellSize)
    {
        gridLayoutGroup.cellSize = gridCellSize;
    }
}
