using System.Collections.Concurrent;
using System.Numerics;

namespace AABB;

public partial class CollisionTester
{
    private static void BufferVsBuffer_Threaded(
        float[] aCenters, float[] aExtents, int aCount,
        float[] bCenters, float[] bExtents, int bCount, List<(int aIndex, int bIndex)> results)
    {
        var ranges = Partitioner.Create(0, aCount);

        Parallel.ForEach(ranges,
            () => GetResultsList(),
            (range, state, localResults) => BufferVsBufferWorker(aCenters, aExtents, bCenters, bExtents, bCount, localResults, range.Item1, range.Item2),
            localResults =>
            {
                lock(results)
                    results.AddRange(localResults);
                ReturnResultsList(localResults);
            });
    }
    
    private void NarrowSweep_Threaded(CollisionLayer bufferA, CollisionLayer bufferB, List<(int aIndex, int bIndex)> results)
    {
        var ranges = Partitioner.Create(0, intersectsX.Count);

        Parallel.ForEach(ranges,
            () => GetResultsList(),
            (range, state, localResults) => NarrowSweepWorker(bufferA, bufferB, localResults, range.Item1, range.Item2),
            localResults =>
            {
                lock(results)
                    results.AddRange(localResults);
                ReturnResultsList(localResults);
            });
    }

    

    
}