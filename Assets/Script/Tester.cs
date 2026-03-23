using Unity.VisualScripting;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private GridObjectDebugVisual gridObjectDebugVisual;

    void Start()
    {
        GridSystem.Instance.CreateDebugObjectVisuals(gridObjectDebugVisual);
    }
}
