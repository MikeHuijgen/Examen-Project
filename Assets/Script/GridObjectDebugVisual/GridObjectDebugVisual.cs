using System;
using TMPro;
using UnityEngine;

public class GridObjectDebugVisual : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI debugTextGridPosition;
    [SerializeField] private TextMeshProUGUI debugTextGridObject;
    private GridObject _gridObject;

    private Func<GridPosition, Vector3> _gridToWorldFunc;

    private static int counter;
    public void Initialize(GridObject gridObject, int width, int height, Func<GridPosition, Vector3> gridToWorldFunc)
    {
        _gridObject = gridObject;
        _gridObject.PositionChangedCallback(UpdateRectPosition);
        rectTransform.sizeDelta = new Vector2(width, height);
        _gridToWorldFunc = gridToWorldFunc;
        UpdateText();
    }

    public void UpdateRectPosition(GridPosition gridPosition)
    {
        rectTransform.anchoredPosition = _gridToWorldFunc(gridPosition);
        debugTextGridPosition.text =     
        "x = " + _gridObject.GetGridPosition.X + "\n" +
        "y = " + _gridObject.GetGridPosition.Y;

    }

    public GridObject GetGridObject => _gridObject;

    private void UpdateText()
    {
        debugTextGridPosition.text =         
        "x = " + _gridObject.GetGridPosition.X + "\n" +
        "y = " + _gridObject.GetGridPosition.Y;
        
        debugTextGridObject.text = counter.ToString();
        counter++;         
    }
}
