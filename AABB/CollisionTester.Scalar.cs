using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    private static void BufferVsBuffer_Scalar(float[] aCenters, float[] aExtents, int aCount, float[] bCenters, float[] bExtents, int bCount, List<(int aIndex, int bIndex)> results)
    {
        (int startIndex, int endIndex) = (0, aCount);
        BufferVsBufferWorker(aCenters, aExtents, bCenters, bExtents, bCount, results, startIndex, endIndex);
    }

    private static List<(int aIndex, int bIndex)> BufferVsBufferWorker(float[] aCenters, float[] aExtents, float[] bCenters, float[] bExtents, int bCount, List<(int aIndex, int bIndex)> results, int startIndex, int endIndex)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            for (var j = 0; j < bCount; j++)
            {
                if (aCenters[i] - aExtents[i] <= bCenters[j] + bExtents[j] && aCenters[i] + aExtents[i] >= bCenters[j] - bExtents[j])
                {
                    results.Add((i, j));
                }
            }
        }

        return results;
    }


    private void NarrowSweep_Scalar(BoxBuffer bufferA, BoxBuffer bufferB, List<(int aIndex, int bIndex)> results)
    {
        NarrowSweepWorker(bufferA, bufferB, results, 0, intersectsX.Count);
    }

    private List<(int aIndex, int bIndex)> NarrowSweepWorker(BoxBuffer bufferA, BoxBuffer bufferB, List<(int aIndex, int bIndex)> results, int startIndex, int endIndex)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            var (dynamicIndex, staticIndex) = intersectsX[i];
            if (bufferA.centerY[dynamicIndex] - bufferA.extentY[dynamicIndex] <= bufferB.centerY[staticIndex] + bufferB.extentY[staticIndex] && bufferA.centerY[dynamicIndex] + bufferA.extentY[dynamicIndex] >= bufferB.centerY[staticIndex] - bufferB.extentY[staticIndex])
            {
                results.Add((dynamicIndex, staticIndex));
            }
        }

        return results;
    }
}