using UnityEngine;
using UnityEngine.UI;

public class GridObjectHolder : MonoBehaviour
{

    private void OnEnable()
    {
        GridSystem.OnNewGridObjectCreated += OnNewGridObjectCreated;
    }


    private void OnDisable()
    {
        GridSystem.OnNewGridObjectCreated -= OnNewGridObjectCreated;        
    }

    private void OnNewGridObjectCreated(Transform gridObjectVisual)
    {
        gridObjectVisual.SetParent(transform, false);

    }
}
