using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    private static void BufferVsBuffer_Scalar(float[] aMin, float[] aMax, int aCount, float[] bMin, float[] bMax, int bCount, List<(int aIndex, int bIndex)> results)
    {
        (int startIndex, int endIndex) = (0, aCount);
        
        BufferVsBufferWorker(aMin, aMax, bMin, bMax, bCount, results, startIndex, endIndex);
    }

    private static List<(int aIndex, int bIndex)> BufferVsBufferWorker(float[] aMin, float[] aMax, float[] bMin, float[] bMax, int bCount, List<(int aIndex, int bIndex)> results, int startIndex, int endIndex)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            var q = FindSearchStartIndex(aMin, bMin, bMax, bCount, i);

            for (var j = q; j < bCount && bMin[j] <= aMax[i]; j++)
            {
                if (aMin[i]  <= bMax[j] && aMax[i] >= bMin[j])
                {
                    results.Add((i, j));
                }
            }
        }
        return results;
    }

    private static int FindSearchStartIndex(float[] aMin, float[] bMin, float[] bMax, int bCount, int i)
    {
        var q = Array.BinarySearch(bMin, 0, bCount, aMin[i]);
        if (q < 0)
        {
            q = ~q;
        }
            
        // go backwards in case previous elements have a width that covers aMin[i]
        while (q > 0 && bMax[q - 1] >= aMin[i])
            q--;
        return q;
    }

    private void NarrowSweep_Scalar(CollisionLayer bufferA, CollisionLayer bufferB, List<(int aIndex, int bIndex)> results)
    {
        NarrowSweepWorker(bufferA, bufferB, results, 0, intersectsX.Count);
    }

    private List<(int aIndex, int bIndex)> NarrowSweepWorker(CollisionLayer bufferA, CollisionLayer bufferB, List<(int aIndex, int bIndex)> results, int startIndex, int endIndex)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            var (dynamicIndex, staticIndex) = intersectsX[i];
            var aMin = bufferA.minY;
            var aMax = bufferA.maxY;
            var bMin = bufferB.minY;
            var bMax = bufferB.maxY;
            if (aMin[dynamicIndex] <= bMax[staticIndex] && aMax[dynamicIndex] >= bMin[staticIndex])
            {
                results.Add((dynamicIndex, staticIndex));
            }
        }
        return results;
    }
}