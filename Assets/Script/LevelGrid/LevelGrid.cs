using System;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;
    private GridSystem _gridSystem;

    private void Awake()
    {
        _gridSystem = new GridSystem(levelGridData.GridWidth, levelGridData.GridHeight, levelGridData.GridCellWidth, levelGridData.GridCellHeight);
        GridObjectUIRoot.OnGridRectReady += rect => _gridSystem.SetRectTransform(rect);
    }

    void OnEnable()
    {
        FingerInputTester.OnNewInputEnded += OnNewInputEnded;
    }


    void OnDisable()
    {
        FingerInputTester.OnNewInputEnded -= OnNewInputEnded;  
        GridObjectUIRoot.OnGridRectReady -= rect => _gridSystem.SetRectTransform(rect);      
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateDebugObjectVisuals(levelGridData.GridObjectDebugVisual);                
    }

    private void OnNewInputEnded(Vector2 beginInputPosition, Vector2 endInputPosition)
    {
        var beginGridPosition = _gridSystem.GetWorldPositionToGridPosition(beginInputPosition);
        var endGridPosition = _gridSystem.GetWorldPositionToGridPosition(endInputPosition);

        if (!_gridSystem.IsValidGridPosition(beginGridPosition) || !_gridSystem.IsValidGridPosition(endGridPosition)) return;
        if (beginGridPosition == endGridPosition) return;

        var dx = Mathf.Abs(beginGridPosition.X - endGridPosition.X);
        var dy = Mathf.Abs(beginGridPosition.Y - endGridPosition.Y);

        if (dx > 1 || dy > 1 || (dx == 0 && dy == 0)) return;


        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);
    }
}
