using UnityEngine;
using System.Collections;

public class Point {

    public int X { get; private set; }
    public int Y { get; private set; }

	public Point()
    {
        X = -1;
        Y = -1;
    }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (!(obj is Point))
        {
            return false;
        }

        return Equals((Point)obj);
    }

    public bool Equals(Point point)
    {
        if (X != point.X)
        {
            return false;
        }

        return (Y == point.Y);
    }

    public override int GetHashCode()
    {
        return X ^ Y;
    }
}