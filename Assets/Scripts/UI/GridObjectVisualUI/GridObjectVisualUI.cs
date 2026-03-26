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

    private void OnEnable()
    {
        CharacterInput.Instance.OnGridPositionSelected += OnGridObjectVisualUISelected;
        CharacterInput.Instance.OnGridPositionDeselected += OnGridObjectVisualUIDeselected;
    }

    private void OnDisable()
    {
        CharacterInput.Instance.OnGridPositionSelected -= OnGridObjectVisualUISelected;   
        CharacterInput.Instance.OnGridPositionDeselected -= OnGridObjectVisualUIDeselected;     
    }

    public GridObject GetGridObject => _gridObject;
    public Vector3 GetRectToWorldTransform() => rectTransform.transform.position;

    private void OnGridObjectVisualUISelected(GridPosition gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        gridVisualOutline.effectColor = Color.limeGreen;
    }
    private void OnGridObjectVisualUIDeselected(GridPosition gridPosition)
    {
        if (_gridObject.GetGridPosition != gridPosition) return;
        gridVisualOutline.effectColor = Color.black;
    }
}
