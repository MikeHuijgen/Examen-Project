using UnityEngine;
using System;

public class GridObjectUIRoot : MonoBehaviour
{    
    public static event Action<RectTransform> OnGridRectReady;

    [SerializeField] private RectTransform gridRect;

    private void Awake() => OnGridRectReady?.Invoke(gridRect);

    private void OnEnable() => GridSystem.OnNewGridObjectCreated += OnNewGridObjectCreated;
    
    private void OnDisable() => GridSystem.OnNewGridObjectCreated -= OnNewGridObjectCreated;        

    private void OnNewGridObjectCreated(Transform gridObjectVisual) => gridObjectVisual.SetParent(transform, false);
}
