using UnityEngine;
using UnityEngine.UI;

public class GridObjectHolder : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private void OnEnable()
    {
        GridSystem.OnNewGridObjectCreated += OnNewGridObjectCreated;
    }


    private void OnDisable()
    {
        GridSystem.OnNewGridObjectCreated -= OnNewGridObjectCreated;        
    }

    private void OnNewGridObjectCreated(Transform gridObjectVisual)
    {
        gridObjectVisual.SetParent(transform, false);
        
    }

    private void OnNewGeneratedGrid(Vector2 gridCellSize) => gridLayoutGroup.cellSize = gridCellSize;
}
