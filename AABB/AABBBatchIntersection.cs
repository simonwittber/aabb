using System.Numerics;

namespace AABB;

public static class AABBBatchIntersection
{
    /// <summary>
    /// Batch AABB-vs-AABB intersection test.
    /// Inputs:
    ///   aCx,aCy  – centers of AABBs in “a” set
    ///   aHx,aHy  – half-width/half-height of AABBs in “a” set
    ///   bCx,bCy, bHx,bHy – same for “b” set
    /// Output:
    ///   results – true if a[i] intersects b[i]
    /// </summary>
    public static void BatchIntersects(
        Span<float> aCx, Span<float> aCy, Span<float> aHx, Span<float> aHy,
        Span<float> bCx, Span<float> bCy, Span<float> bHx, Span<float> bHy,
        Span<bool>  results)
    {
        int n     = Math.Min(results.Length, Math.Min(aCx.Length, bCx.Length));
        int width = Vector<float>.Count;
        int i     = 0;

        // vectorized loop
        for (; i + width <= n; i += width)
        {
            // load vectors
            var acx = new Vector<float>(aCx.Slice(i, width));
            var acy = new Vector<float>(aCy.Slice(i, width));
            var ahx = new Vector<float>(aHx.Slice(i, width));
            var ahy = new Vector<float>(aHy.Slice(i, width));

            var bcx = new Vector<float>(bCx.Slice(i, width));
            var bcy = new Vector<float>(bCy.Slice(i, width));
            var bhx = new Vector<float>(bHx.Slice(i, width));
            var bhy = new Vector<float>(bHy.Slice(i, width));

            // compute mins/maxs
            var aMinX = acx - ahx;
            var aMaxX = acx + ahx;
            var aMinY = acy - ahy;
            var aMaxY = acy + ahy;

            var bMinX = bcx - bhx;
            var bMaxX = bcx + bhx;
            var bMinY = bcy - bhy;
            var bMaxY = bcy + bhy;

            // four overlap tests
            var testX1 = Vector.LessThanOrEqual(aMinX, bMaxX);
            var testX2 = Vector.GreaterThanOrEqual(aMaxX, bMinX);
            var testY1 = Vector.LessThanOrEqual(aMinY, bMaxY);
            var testY2 = Vector.GreaterThanOrEqual(aMaxY, bMinY);

            // combine all four with bitwise-and
            var overlap = testX1 & testX2 & testY1 & testY2;

            // write results
            for (int j = 0; j < width; j++)
                results[i + j] = overlap[j] != 0;
        }

        // handle remaining elements scalar‐style
        for (; i < n; i++)
        {
            bool hitX = (aCx[i] - aHx[i] <= bCx[i] + bHx[i]) &&
                        (aCx[i] + aHx[i] >= bCx[i] - bHx[i]);
            bool hitY = (aCy[i] - aHy[i] <= bCy[i] + bHy[i]) &&
                        (aCy[i] + aHy[i] >= bCy[i] - bHy[i]);
            results[i] = hitX && hitY;
        }
    }
}