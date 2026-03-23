public struct GridPosition
{
    public int X;
    public int Y;

    public GridPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"X = {X} Y = {Y}";
    }
}
