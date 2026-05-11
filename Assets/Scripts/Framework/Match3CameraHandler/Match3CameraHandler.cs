using UnityEngine;
using UnityEngine.Serialization;

public class Match3CameraHandler : MonoBehaviour
{
    [SerializeField] private LevelGridData levelGridData;
    
    [SerializeField] private float zoom = 3.5f;
    
    [SerializeField] private Vector2 positionOffset;

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

        float gridWorldWidth = levelGridData.GridWidth * levelGridData.GridCellWidth;

        float calculatedSize = gridWorldWidth / aspectRatio / zoom;

        CameraHolder.Match3Camera.orthographicSize = calculatedSize + 1.5f;
        
        CameraHolder.Match3Camera.transform.position += new Vector3(positionOffset.x, positionOffset.y, 0f);
    }
}
