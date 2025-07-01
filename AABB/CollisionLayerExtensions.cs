using System.Numerics;

namespace AABB;

public static class CollisionLayerExtensions
{
    public static List<int> HitTest(this CollisionLayer layer, Vector2 position)
    {
        var xResults = new List<int>();
        var q = FindSearchStartIndex(layer.minX, layer.maxX, layer.Count, position.X);
        // x sweep
        for (var j = q; j < layer.Count && layer.minX[j] <= position.X; j++)
        {
            if (position.X <= layer.maxX[j] && position.X >= layer.minX[j])
            {
                xResults.Add(j);
            }
        }
        // y sweep
        var yResults = new List<int>();
        foreach (var i in xResults)
        {
            if (position.Y <= layer.maxY[i] && position.Y >= layer.minY[i])
                yResults.Add(i);
        }
        return yResults;
    }

    private static int FindSearchStartIndex(float[] bMin, float[] bMax, int bCount, float aMinValue)
    {
        var q = Array.BinarySearch(bMin, 0, bCount, aMinValue);
        if (q < 0)
        {
            q = ~q;
        }

        // go backwards in case previous elements have a width that covers aMin[i]
        while (q > 0 && bMax[q - 1] >= aMinValue)
            q--;
        return q;
    }
}