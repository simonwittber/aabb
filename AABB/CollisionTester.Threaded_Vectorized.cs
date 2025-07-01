using System.Collections.Concurrent;
using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    static Stack<List<(int aIndex, int bIndex)>> resultsPool = new();

    static List<(int aIndex, int bIndex)> GetResultsList()
    {
        lock (resultsPool)
        {
            if (resultsPool.Count > 0)
                return resultsPool.Pop();
            return new List<(int aIndex, int bIndex)>();
        }
    }
    
    static void ReturnResultsList(List<(int aIndex, int bIndex)> results)
    {
        results.Clear();
        lock(resultsPool)
        {
            resultsPool.Push(results);
        }
    }
    
    private static void BufferVsBuffer_Threaded_Vectorized(
        float[] aMin, float[] aMax, int aCount,
        float[] bMin, float[] bMax, int bCount, List<(int aIndex, int bIndex)> results)
    {
        
        Parallel.For(
            0, aCount,
            () => GetResultsList(),
            (i, state, localResults) => BufferVsBuffer_Threaded_Vectorized_Worker(aMin, aMax, i, bMin, bMax, bCount, localResults),
            localResults =>
            {
                lock(results)
                    results.AddRange(localResults);
                ReturnResultsList(localResults);
            });
    }

    private static List<(int aIndex, int bIndex)> BufferVsBuffer_Threaded_Vectorized_Worker(float[] aMin, float[] aMax, int i, float[] bMin, float[] bMax, int bCount, List<(int aIndex, int bIndex)> results)
    {
        var W = Vector<float>.Count;

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


        return results;
    }

    private void NarrowSweep_Vectorized_Threaded(CollisionLayer bufferA, CollisionLayer bufferB, List<(int aIndex, int bIndex)> results)
    {
        var ranges = Partitioner.Create(0, intersectsX.Count);

        Parallel.ForEach(ranges,
            () => GetResultsList(),
            (range, state, localResults) => NarrowSweep_Vectorized_Threaded_Worker(bufferA, bufferB, range, localResults),
            localResults =>
            {
                lock(results)
                    results.AddRange(localResults);
                ReturnResultsList(localResults);
            });
    }

    private List<(int aIndex, int bIndex)> NarrowSweep_Vectorized_Threaded_Worker(CollisionLayer bufferA, CollisionLayer bufferB, Tuple<int, int> range, List<(int aIndex, int bIndex)> localResults)
    {
        int W = Vector<float>.Count;
        int n = range.Item2;
        int idx = range.Item1;

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
                    localResults.Add(intersectsX[idx + k]);

            idx += W;
        }

        // scalar remainder
        for (; idx < n; idx++)
        {
            var (dynamicIndex, staticIndex) = intersectsX[idx];
            if (bufferA.minY[dynamicIndex] <= bufferB.maxY[staticIndex] && bufferA.maxY[dynamicIndex] >= bufferB.minY[staticIndex])
            {
                localResults.Add((dynamicIndex, staticIndex));
            }
        }

        return localResults;
    }
}