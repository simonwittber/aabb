using System.Numerics;

namespace AABB;

public struct Box : IEquatable<Box>
{
    public Vector2 center;
    public Vector2 extents;

    public Box(float x, float y, float w, float h)
    {
        this.center = new Vector2(x, y);
        this.extents = new Vector2(w/2f, h/2f);
    }
    
    public float xMin => center.X - extents.X;
    public float xMax => center.X + extents.X;
    public float yMin => center.Y - extents.Y;
    public float yMax => center.Y + extents.Y;

    public static bool Intersects(Box a, Box b)
    {
        return a.xMin <= b.xMax &&
               a.xMax >= b.xMin &&
               a.yMin <= b.yMax &&
               a.yMax >= b.yMin;
    }

    public bool Equals(Box other)
    {
        return center.Equals(other.center) && extents.Equals(other.extents);
    }

    public override bool Equals(object? obj)
    {
        return obj is Box other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(center, extents);
    }

    public static bool operator ==(Box left, Box right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Box left, Box right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Box({center}, {extents})";
    }
}