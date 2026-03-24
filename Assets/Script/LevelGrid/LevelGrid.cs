using System;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private int gridCellWidth = 100;
    [SerializeField] private int gridCellHeight = 100;
    [SerializeField] private RectTransform gridRectTransform;
    [SerializeField] private GridObjectDebugVisual gridObjectDebugVisual;
    private GridSystem _gridSystem;

    private void Awake()
    {
        _gridSystem = new GridSystem(gridWidth, gridHeight, gridCellWidth, gridCellHeight, gridRectTransform);
    }

    void OnEnable()
    {
        FingerInputTester.OnNewInputEnded += OnNewInputEnded;
    }


    void OnDisable()
    {
        FingerInputTester.OnNewInputEnded -= OnNewInputEnded;        
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateDebugObjectVisuals(gridObjectDebugVisual);                
    }

    private void OnNewInputEnded(Vector2 beginInputPosition, Vector2 endInputPosition)
    {
        var beginGridPosition = _gridSystem.GetWorldPositionToGridPosition(beginInputPosition);
        var endGridPosition = _gridSystem.GetWorldPositionToGridPosition(endInputPosition);

        if (!_gridSystem.IsValidGridPosition(beginGridPosition) || !_gridSystem.IsValidGridPosition(endGridPosition)) return;
        if (beginGridPosition == endGridPosition) return;

        var gridObjectA = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var gridObjectB = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        _gridSystem.SwapGridObjects(gridObjectA, gridObjectB);

        print($"Before swap: A: {beginGridPosition}, B: {endGridPosition}");
        print($"After swap:  A: {gridObjectA.GetGridPosition}, B: {gridObjectB.GetGridPosition}");
    }
}
