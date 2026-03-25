using UnityEngine;

[CreateAssetMenu(fileName = "LevelGridData", menuName = "Scriptable Objects/LevelGridData")]
public class LevelGridData : ScriptableObject
{
    public int GridWidth = 7;
    public int GridHeight = 7;
    public int GridCellWidth = 100;
    public int GridCellHeight = 100;
    public GridObjectDebugVisual GridObjectDebugVisual;
}
