using System.Numerics;

namespace AABB;

public struct Box
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
}