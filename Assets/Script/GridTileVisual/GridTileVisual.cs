using TMPro;
using UnityEngine;

public class GridTileVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    private GridTileData _gridTileData;
    public void Initialize(GridTileData gridTileData)
    {
        _gridTileData = gridTileData;
        debugText.text = _gridTileData.GetGridTilePosition.ToString();
    }

    public GridTileData GetGridTileData => _gridTileData;
}
