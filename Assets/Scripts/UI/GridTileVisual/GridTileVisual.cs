using UnityEngine;
using UnityEngine.UI;

public class GridTileVisual : MonoBehaviour
{
    [SerializeField] private Image border;
    private GridPosition _gridTileVisualPosition;

    public void SetGridTileVisualPosition(GridPosition gridPosition)
    {
        _gridTileVisualPosition = gridPosition;
        LevelGrid.OnTileSelected += OnTileSelected;
        LevelGrid.OnTileDeselected += OnTileDeselected; 
    }

    void OnDisable()
    {
        LevelGrid.OnTileSelected -= OnTileSelected;
        LevelGrid.OnTileDeselected -= OnTileDeselected;        
    }

    private void OnTileSelected(GridPosition? targetGridPosition)
    {
        if (_gridTileVisualPosition != targetGridPosition) return;
        border.color = Color.limeGreen;
    }
    private void OnTileDeselected(GridPosition? targetGridPosition)
    {
        if (_gridTileVisualPosition != targetGridPosition) return;
        border.color = Color.black;
    }
}
