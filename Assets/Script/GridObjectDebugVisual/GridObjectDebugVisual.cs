using TMPro;
using UnityEngine;

public class GridObjectDebugVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugTextGridPosition;
    [SerializeField] private TextMeshProUGUI debugTextGridObject;
    private GridObject _gridObject;
    public void Initialize(GridObject gridObject)
    {
        _gridObject = gridObject;
        debugTextGridPosition.text = _gridObject.GetGridTilePosition.ToString();
        debugTextGridObject.text = _gridObject.GetGridTilePosition.Y.ToString();
    }

    public GridObject GetGridObject => _gridObject;
}
