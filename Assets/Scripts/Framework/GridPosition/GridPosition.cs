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

    public override string ToString() => $"X = {X} Y = {Y}";
    public static bool operator ==(GridPosition a , GridPosition b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(GridPosition a , GridPosition b) => !(a == b);
    public override bool Equals(object obj) => obj is GridPosition position && X == position.X && Y == position.Y;
    public override int GetHashCode() => HashCode.Combine(X,Y);
}