using System;

public struct gridObject
{
    public int X;
    public int Y;

    public gridObject(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"X = {X} Y = {Y}";
    }

    public static bool operator ==(gridObject a , gridObject b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(gridObject a , gridObject b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is gridObject position &&
                X == position.X &&
                Y == position.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X,Y);
    }
}