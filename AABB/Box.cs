using System.Numerics;

namespace AABB;

public struct Box : IEquatable<Box>
{
    public int id { get; private set; }
    public Vector2 center;
    public Vector2 extents;

    public Box(int id, float centerX, float centerY, float width, float height)
    {
        this.id = id;
        this.center = new Vector2(centerX, centerY);
        this.extents = new Vector2(width/2f, height/2f);
    }

    public Box(int id, Vector2 center, Vector2 size)
    {
        this.id = id;
        this.center = center;
        this.extents = size / 2f;
    }

    public static Box FromMinMax(int id, Vector2 min, Vector2 max)
    {
        var center = (min + max) / 2f;
        var size = Vector2.Abs(max - min);
        return new Box(id, center, size);
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
        return id.Equals(other.id) && center.Equals(other.center) && extents.Equals(other.extents);
    }

    public override bool Equals(object? obj)
    {
        return obj is Box other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id, center, extents);
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
        return $"Box({id}, {center}, {extents})";
    }
}