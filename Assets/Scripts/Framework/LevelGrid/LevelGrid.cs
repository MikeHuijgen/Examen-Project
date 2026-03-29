using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private List<GridObjectVisual> gridObjectVisuals = new List<GridObjectVisual>();
    private GridSystem _gridSystem;
    private GridPosition? _beginTouchGridPosition;
    private GridPosition? _currentSelectedGridPosition;
    private GridPosition? _endSelectedGridPosition;


    private void Awake()
    {
        _gridSystem = new GridSystem(
            levelGridData.GridWidth, 
            levelGridData.GridHeight, 
            levelGridData.GridCellWidth, 
            levelGridData.GridCellHeight,
            levelGridData.swapTolerance);
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

        CharacterInput.Instance.OnNewFingerDownInput += OnNewFingerDownInput;
        CharacterInput.Instance.OnNewFingerUpInput += OnNewFingerUpInput;
    }

    private void OnNewFingerDownInput(Vector2 fingerPosition)
    {
        var newGridPosition = _gridSystem.ConvertWorldPositionToGridPosition(fingerPosition);
        if (!_gridSystem.IsValidGridPosition(newGridPosition)) return;

        _beginTouchGridPosition = newGridPosition;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        var newGridPosition = _gridSystem.ConvertWorldPositionToGridPosition(fingerPosition);
        if (!_gridSystem.IsValidGridPosition(newGridPosition)) {_currentSelectedGridPosition = null; return;}

        _endSelectedGridPosition = newGridPosition;

        if (_beginTouchGridPosition == _endSelectedGridPosition && _currentSelectedGridPosition == null)
        {
            _currentSelectedGridPosition = newGridPosition;
            return;
        }

        if (_currentSelectedGridPosition != null && _beginTouchGridPosition == _endSelectedGridPosition)
        {
            print("Click move");
        }
        else
        {
            print("Swipe move");
        }


        _currentSelectedGridPosition = null;
    }

    private void OnRequestGridObjectSwap(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        var dx = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var dy = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        if (dx + dy != 1) return;

        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);        
    }
}
