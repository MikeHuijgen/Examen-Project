using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectVisualUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image border;
    private GridObject _gridObject;

    private Func<gridObject, Vector3> _gridToWorldFunc;

    public void Initialize(GridObject gridObject, int width, int height, Func<gridObject, Vector3> gridToWorldFunc)
    {
        _gridObject = gridObject;
        _gridObject.OnPositionChanged += OnPositionChanged;
        rectTransform.sizeDelta = new Vector2(width, height);
        _gridToWorldFunc = gridToWorldFunc;
    }

    public void OnPositionChanged()
    {
        var gridPosition = _gridObject.GetGridPosition;
        rectTransform.anchoredPosition = _gridToWorldFunc(gridPosition);
    }

    public GridObject GetGridObject => _gridObject;
    public Vector3 GetRectToWorldTransform() => rectTransform.transform.position;

    void OnEnable()
    {
        LevelGrid.OnTileSelected += OnTileSelected;
        LevelGrid.OnTileDeselected += OnTileDeselected;
    }

    void OnDisable()
    {
        LevelGrid.OnTileSelected -= OnTileSelected;
        LevelGrid.OnTileDeselected -= OnTileDeselected;        
    }

    private void OnTileSelected(gridObject? gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        border.color = Color.limeGreen;
    }
    private void OnTileDeselected(gridObject? gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        border.color = Color.black;
    }
}
