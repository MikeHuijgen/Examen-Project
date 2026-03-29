public struct GridHit
{
    public GridPosition hitGridPosition;
    public float rawX;
    public float rawY;

    public GridHit (GridPosition gridPosition, float rawX, float rawY)
    {
        hitGridPosition = gridPosition;
        this.rawX = rawX;
        this.rawY = rawY;
    }
}
