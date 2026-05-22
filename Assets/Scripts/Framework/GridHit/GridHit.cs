using UnityEngine;

public struct GridHit
{
    public GridPosition HitGridPosition;
    public float RawX;
    public float RawY;
    public Vector2 LocalPos; 

    public GridHit (GridPosition gridPosition, float rawX, float rawY, Vector2 localPos)
    {
        HitGridPosition = gridPosition;
        RawX = rawX;
        RawY = rawY;
        LocalPos = localPos;
    }
}
