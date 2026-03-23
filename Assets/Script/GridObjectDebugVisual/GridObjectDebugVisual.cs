using TMPro;
using UnityEngine;

public class GridObjectDebugVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    private GridObject _gridTileData;
    public void Initialize(GridObject gridTileData)
    {
        _gridTileData = gridTileData;
        debugText.text = _gridTileData.GetGridTilePosition.ToString();
    }

    public GridObject GetGridTileData => _gridTileData;
}
