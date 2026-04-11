using UnityEngine;

[CreateAssetMenu(fileName = "LevelGridData", menuName = "Scriptable Objects/LevelGridData")]
public class LevelGridData : ScriptableObject
{
    public int GridWidth = 7;
    public int GridHeight = 7;
    public int GridCellWidth = 100;
    public int GridCellHeight = 100;
    [Range(0, 1)] public float ClickTolerance = .45f;
    [Range(0, 1)] public float SwipeDirectionTolerance = .15f;
    [Range(0, 1)] public float SwipeMaxDiagonalDeviation = .45f;
    public float VisualSwapSpeed = .15f;
    public float VisualFallSpeed = .25f;
    public Transform GridTileVisual;
}
