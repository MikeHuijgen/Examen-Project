using TMPro;
using UnityEngine;

public class GridObjectDebugVisual : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI debugTextGridPosition;
    [SerializeField] private TextMeshProUGUI debugTextGridObject;
    private GridObject _gridObject;
    public void Initialize(GridObject gridObject, int width, int height)
    {
        _gridObject = gridObject;
        debugTextGridPosition.text = _gridObject.GetGridTilePosition.ToString();
        debugTextGridObject.text = _gridObject.GetGridTilePosition.Y.ToString();
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    public GridObject GetGridObject => _gridObject;
}
