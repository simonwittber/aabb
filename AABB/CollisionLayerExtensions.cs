using System.Numerics;

namespace AABB;

public static class CollisionLayerExtensions
{
    public static void HitTest(this CollisionLayer layer, Vector2 position, List<int> results)
    {
        float x = position.X, y = position.Y;
        int n = layer.Count;
        var minX = layer.minX;
        var maxX = layer.maxX;
        var minY = layer.minY;
        var maxY = layer.maxY;

        var q = FindSearchStartIndex(minX, maxX, n, x);
        // x sweep
        for (int j = q; j < n && minX[j] <= x; j++)
        {
            // X-test
            if (x < minX[j] || x > maxX[j]) 
                continue;

            // Y-test immediately
            if (y >= minY[j] && y <= maxY[j])
                results.Add(j);
        }
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

    public static void HitTestVectorized(this CollisionLayer layer, Vector2 position, List<int> results)
    {
        float x = position.X, y = position.Y;
        int n = layer.Count;
        var minX = layer.minX;
        var maxX = layer.maxX;
        var minY = layer.minY;
        var maxY = layer.maxY;

        // find the first index where minX[i] <= x
        int i = FindSearchStartIndex(minX, maxX, n, x);

        int W = Vector<float>.Count;
        var xVec = new Vector<float>(x);
        var yVec = new Vector<float>(y);

        // vectorized main loop
        for (; i + W <= n && minX[i] <= x; i += W)
        {
            // quick block-out: if the first lane already starts past x, we're done
            if (minX[i] > x) break;

            // load W lanes at once
            var vMinX = new Vector<float>(minX, i);
            var vMaxX = new Vector<float>(maxX, i);
            var vMinY = new Vector<float>(minY, i);
            var vMaxY = new Vector<float>(maxY, i);

            // test X and Y overlaps
            var mask = 
                Vector.LessThanOrEqual(vMinX, xVec)
                & Vector.GreaterThanOrEqual(vMaxX, xVec)
                & Vector.LessThanOrEqual(vMinY, yVec)
                & Vector.GreaterThanOrEqual(vMaxY, yVec);

            // extract any hits in this block
            for (int k = 0; k < W; k++)
            {
                if (mask[k] != 0)
                    results.Add(i + k);
            }
        }

        // scalar tail
        for (; i < n && minX[i] <= x; i++)
        {
            if (x >= minX[i] && x <= maxX[i] && y >= minY[i] && y <= maxY[i])
            {
                results.Add(i);
            }
        }
    }
}