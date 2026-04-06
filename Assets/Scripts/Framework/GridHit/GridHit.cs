using UnityEngine;

public struct GridHit
{
    public gridObject hitGridPosition;
    public float rawX;
    public float rawY;
    public Vector2 localPos; 

    public GridHit (gridObject gridPosition, float rawX, float rawY, Vector2 localPos)
    {
        hitGridPosition = gridPosition;
        this.rawX = rawX;
        this.rawY = rawY;
        this.localPos = localPos;
    }
}
