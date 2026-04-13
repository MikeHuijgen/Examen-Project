using UnityEngine;
using UnityEngine.UI;

public class GridTileVisual : MonoBehaviour
{
    [SerializeField] private Image border;

    public void OnTileSelected() => border.color = Color.limeGreen;
    public void OnTileDeselected() => border.color = Color.black;
}
