using System;

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

    public static bool operator ==(GridPosition a , GridPosition b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(GridPosition a , GridPosition b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
                X == position.X &&
                Y == position.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X,Y);
    }
}