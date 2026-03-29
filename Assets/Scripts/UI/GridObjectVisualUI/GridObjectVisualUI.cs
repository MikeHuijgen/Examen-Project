using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectVisualUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI debugTextGridPosition;
    [SerializeField] private TextMeshProUGUI debugTextGridObject;
    [SerializeField] private Outline gridVisualOutline;
    private GridObject _gridObject;

    private Func<GridPosition, Vector3> _gridToWorldFunc;
    private static int counter;

    public void Initialize(GridObject gridObject, int width, int height, Func<GridPosition, Vector3> gridToWorldFunc)
    {
        _gridObject = gridObject;
        _gridObject.OnPositionChanged += OnPositionChanged;
        rectTransform.sizeDelta = new Vector2(width, height);
        _gridToWorldFunc = gridToWorldFunc;
        UpdateText();
    }

    public void OnPositionChanged()
    {
        var gridPosition = _gridObject.GetGridPosition;
        rectTransform.anchoredPosition = _gridToWorldFunc(gridPosition);
        debugTextGridPosition.text =     
        "x = " + _gridObject.GetGridPosition.X + "\n" +
        "y = " + _gridObject.GetGridPosition.Y;

    }


    private void UpdateText()
    {
        debugTextGridPosition.text =         
        "x = " + _gridObject.GetGridPosition.X + "\n" +
        "y = " + _gridObject.GetGridPosition.Y;
        
        debugTextGridObject.text = counter.ToString();
        counter++;         
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

    private void OnTileSelected(GridPosition? gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        gridVisualOutline.effectColor = Color.limeGreen;
    }
    private void OnTileDeselected(GridPosition? gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        gridVisualOutline.effectColor = Color.black;
    }
}
