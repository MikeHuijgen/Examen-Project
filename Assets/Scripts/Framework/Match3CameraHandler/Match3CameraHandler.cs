using UnityEngine;

public class Match3CameraHandler : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;

    private void Start()
    {
        var x = levelGridData.GridWidth * levelGridData.GridCellWidth / 2;
        var y = levelGridData.GridHeight * levelGridData.GridCellHeight / 2;
        transform.position = new Vector3(x,y, transform.position.z);
    }
}
