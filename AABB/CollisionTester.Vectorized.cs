using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    private static void BufferVsBuffer_Vectorized(float[] aCenters, float[] aExtents, int aCount, float[] bCenters, float[] bExtents, int bCount, List<(int aIndex, int bIndex)> results)
    {
        var W = Vector<float>.Count;
        (int startIndex, int endIndex) = (0, aCount);
        for (var i = startIndex; i < endIndex; i++)
        {
            // build the scalar AABB[i] min/max as a Vector
            var center = new Vector<float>(aCenters[i]);
            var extent = new Vector<float>(aExtents[i]);
            var min = center - extent;
            var max = center + extent;

            var j = 0;
            // vectorized inner loop
            for (; j + W <= bCount; j += W)
            {
                var testCenter = new Vector<float>(bCenters.AsSpan(j, W));
                var testExtent = new Vector<float>(bExtents.AsSpan(j, W));
                var testMin = testCenter - testExtent;
                var testMax = testCenter + testExtent;

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
            for (; j < bCount; j++)
            {
                if (aCenters[i] - aExtents[i] <= bCenters[j] + bExtents[j] && aCenters[i] + aExtents[i] >= bCenters[j] - bExtents[j])
                {
                    // Mark intersection
                    results.Add((i, j));
                }
            }
        }
    }
    
    private void NarrowSweep_Vectorized(BoxBuffer bufferA, BoxBuffer bufferB, List<(int aIndex, int bIndex)> results)
    {
        
        int W = Vector<float>.Count;
        int n = intersectsX.Count;
        int idx = 0;

        // temporary SoA buffers for one Vector-width of pairs
        Span<float> centerTmp = stackalloc float[W];
        Span<float> extentTmp = stackalloc float[W];
        Span<float> testCenter = stackalloc float[W];
        Span<float> testExtent = stackalloc float[W];

        // Vector-ized Y-axis test on each block of W candidate pairs
        while (idx + W <= n)
        {
            // gather Y data for lanes [0..W)
            for (int k = 0; k < W; k++)
            {
                var (dynamicIndex, staticIndex) = intersectsX[idx + k];
                centerTmp[k] = bufferA.centerY[dynamicIndex];
                extentTmp[k] = bufferA.extentY[dynamicIndex];
                testCenter[k] = bufferB.centerY[staticIndex];
                testExtent[k] = bufferB.extentY[staticIndex];
            }

            // build A’s min/max
            var center = new Vector<float>(centerTmp);
            var extent = new Vector<float>(extentTmp);
            var min = center - extent;
            var max = center + extent;

            // build B’s min/max
            var tCenter = new Vector<float>(testCenter);
            var tExtent = new Vector<float>(testExtent);
            var tMin = tCenter - tExtent;
            var tMax = tCenter + tExtent;

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
            if (bufferA.centerY[dynamicIndex] - bufferA.extentY[dynamicIndex] <= bufferB.centerY[staticIndex] + bufferB.extentY[staticIndex] && bufferA.centerY[dynamicIndex] + bufferA.extentY[dynamicIndex] >= bufferB.centerY[staticIndex] - bufferB.extentY[staticIndex])
            {
                results.Add((dynamicIndex, staticIndex));
            }
        }
    }
}