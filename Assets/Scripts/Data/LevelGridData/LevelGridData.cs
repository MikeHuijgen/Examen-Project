using UnityEngine;

[CreateAssetMenu(fileName = "LevelGridData", menuName = "Scriptable Objects/LevelGridData")]
public class LevelGridData : ScriptableObject
{
    public int GridWidth = 7;
    public int GridHeight = 7;
    public int GridCellWidth = 100;
    public int GridCellHeight = 100;
    [Range(0, 1)] public float ClickTolerance = .4f;
    [Range(0, 1)] public float SwipeDirectionTolerance = .4f;
    [Range(1, 2)] public float SwipeMaxDiagonalDeviation = 1.2f;
    public GridObjectVisualUI GridObjectDebugVisual;
}
