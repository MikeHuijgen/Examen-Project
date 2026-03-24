using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private int gridCellWidth = 100;
    [SerializeField] private int gridCellHeight = 100;
    [SerializeField] private RectTransform gridRectTransform;
    [SerializeField] private GridObjectDebugVisual gridObjectDebugVisual;
    private GridSystem _gridSystem;

    private void Awake()
    {
        _gridSystem = new GridSystem(gridWidth, gridHeight, gridCellWidth, gridCellHeight, gridRectTransform);
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateDebugObjectVisuals(gridObjectDebugVisual);                
    }
}
