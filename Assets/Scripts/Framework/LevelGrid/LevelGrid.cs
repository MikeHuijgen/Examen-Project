using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private List<GridObjectVisual> gridObjectVisuals = new List<GridObjectVisual>();
    private GridSystem _gridSystem;

    private void Awake()
    {
        _gridSystem = new GridSystem(levelGridData.GridWidth, levelGridData.GridHeight, levelGridData.GridCellWidth, levelGridData.GridCellHeight);
        GridObjectUIRoot.OnGridRectReady += rect => _gridSystem.SetRectTransform(rect);
    }

    void OnDisable()
    {
        GridObjectUIRoot.OnGridRectReady -= rect => _gridSystem.SetRectTransform(rect);      
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridObjectVisualUIs(levelGridData.GridObjectDebugVisual);  

        _gridSystem.CreateGridObjectVisuals(gridObjectVisuals);     

        CharacterInput.Instance.SetOnRequestGridObjectSwapCallback(OnRequestGridObjectSwap);
        CharacterInput.Instance.SetIsValidGridPositionCallback(IsValidGridPosition);   
    }

    private void OnRequestGridObjectSwap(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var dx = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var dy = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        if (dx > 1 || dy > 1 || (dx == 0 && dy == 0)) return;

        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);        
    }

    private GridPosition? IsValidGridPosition(Vector2 worldPosition)
    {
        var gridPosition = _gridSystem.GetWorldPositionToGridPosition(worldPosition);
        var isValidGridPosition = _gridSystem.IsValidGridPosition(gridPosition);

        return isValidGridPosition ? gridPosition : null;
    }
}
