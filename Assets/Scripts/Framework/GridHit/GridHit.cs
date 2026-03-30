using UnityEngine;

public struct GridHit
{
    public GridPosition hitGridPosition;
    public float rawX;
    public float rawY;
    public Vector2 localPos; 

    public GridHit (GridPosition gridPosition, float rawX, float rawY, Vector2 localPos)
    {
        hitGridPosition = gridPosition;
        this.rawX = rawX;
        this.rawY = rawY;
        this.localPos = localPos;
    }
}
