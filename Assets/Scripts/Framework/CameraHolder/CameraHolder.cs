using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public static Camera Match3Camera {get; private set;}
    [SerializeField] private Camera match3Camera;

    private void Awake() => Match3Camera = match3Camera;
}
