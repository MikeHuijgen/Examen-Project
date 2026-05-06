using UnityEngine;

public class Match3CameraHandler : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;

    private void Start()
    {
        AdjustTransformPosition();
        AdjustCamera();
    }

    private void AdjustTransformPosition()
    {
        var x = levelGridData.GridWidth * levelGridData.GridCellWidth / 2;
        var y = levelGridData.GridHeight * levelGridData.GridCellHeight / 2;
        transform.position = new Vector3(x,y, transform.position.z);        
    }

    private void AdjustCamera()
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        CameraHolder.Match3Camera.orthographicSize = levelGridData.GridWidth * levelGridData.GridCellWidth / aspectRatio / 3f + 1.5f;
    }
}
