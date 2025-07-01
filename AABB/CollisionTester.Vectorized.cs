using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    private static void BufferVsBuffer_Vectorized(float[] aMin, float[] aMax, int aCount, float[] bMin, float[] bMax, int bCount, List<(int aIndex, int bIndex)> results)
    {
        var W = Vector<float>.Count;
        (int startIndex, int endIndex) = (0, aCount);
        
        for (var i = startIndex; i < endIndex; i++)
        {
            // build the scalar AABB[i] min/max as a Vector
            var min = new Vector<float>(aMin[i]);
            var max = new Vector<float>(aMax[i]);
            
            var q = FindSearchStartIndex(bMin, bMax, bCount, aMin[i]);
            var j = q;
            // vectorized inner loop
            for (; j + W <= bCount && bMin[j] <= aMax[i]; j += W)
            {
                var testMin = new Vector<float>(bMin.AsSpan(j, W));
                var testMax = new Vector<float>(bMax.AsSpan(j, W));

                // overlap on X if aMin <= bMax && aMax >= bMin
                var mask = Vector.LessThanOrEqual(min, testMax) & Vector.GreaterThanOrEqual(max, testMin);

                // extract set lanes
                for (var k = 0; k < W; k++)
                {
                    if (mask[k] != 0)
                    {
                        // Mark intersection
                        results.Add((i, j + k));
                    }
                }
            }

            // scalar remainder that could not be vectorized
            for (; j < bCount && bMin[j] <= aMax[i]; j++)
            {
                if (aMin[i] <= bMax[j] && aMax[i] >= bMin[j])
                {
                    // Mark intersection
                    results.Add((i, j));
                }
            }
        }
    }
    
    private void NarrowSweep_Vectorized(CollisionLayer bufferA, CollisionLayer bufferB, List<(int aIndex, int bIndex)> results)
    {
        
        int W = Vector<float>.Count;
        int n = intersectsX.Count;
        int idx = 0;

        // temporary SoA buffers for one Vector-width of pairs
        Span<float> aMin = stackalloc float[W];
        Span<float> aMax = stackalloc float[W];
        Span<float> bMin = stackalloc float[W];
        Span<float> bMax = stackalloc float[W];

        // Vector-ized Y-axis test on each block of W candidate pairs
        while (idx + W <= n)
        {
            // gather Y data for lanes [0..W)
            for (int k = 0; k < W; k++)
            {
                var (dynamicIndex, staticIndex) = intersectsX[idx + k];
                aMin[k] = bufferA.minY[dynamicIndex];
                aMax[k] = bufferA.maxY[dynamicIndex];
                bMin[k] = bufferB.minY[staticIndex];
                bMax[k] = bufferB.maxY[staticIndex];
            }

            // build A’s min/max
            var min = new Vector<float>(aMin);
            var max = new Vector<float>(aMax);

            // build B’s min/max
            var tMin = new Vector<float>(bMin);
            var tMax = new Vector<float>(bMax);

            // single‐axis overlap test on Y
            var mask = Vector.LessThanOrEqual(min, tMax) & Vector.GreaterThanOrEqual(max, tMin);

            // extract matching lanes
            for (int k = 0; k < W; k++)
                if (mask[k] != 0)
                    results.Add(intersectsX[idx + k]);

            idx += W;
        }

        // scalar remainder
        for (; idx < n; idx++)
        {
            var (dynamicIndex, staticIndex) = intersectsX[idx];
            if (bufferA.minY[dynamicIndex] <= bufferB.maxY[staticIndex] && bufferA.maxY[dynamicIndex] >= bufferB.minY[staticIndex])
            {
                results.Add((dynamicIndex, staticIndex));
            }
        }
    }
}