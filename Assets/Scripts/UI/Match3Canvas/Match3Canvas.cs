using UnityEngine;

public class Match3Canvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Start() => canvas.worldCamera = CameraHolder.Match3Camera;
}
