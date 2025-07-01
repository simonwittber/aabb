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
        var allResults = new ConcurrentBag<List<(int, int)>>();

        Parallel.ForEach(ranges,
            () => new List<(int aIndex, int bIndex)>(),
            (range, state, localResults) => BufferVsBufferWorker(aCenters, aExtents, bCenters, bExtents, bCount, localResults, range.Item1, range.Item2),
            localResults => { allResults.Add(localResults); });
        foreach (var i in allResults) results.AddRange(i);
    }
    
    private void NarrowSweep_Threaded(BoxBuffer bufferA, BoxBuffer bufferB, List<(int aIndex, int bIndex)> results)
    {
        var ranges = Partitioner.Create(0, intersectsX.Count);
        var allResults = new ConcurrentBag<List<(int, int)>>();

        Parallel.ForEach(ranges,
            () => new List<(int aIndex, int bIndex)>(),
            (range, state, localResults) => NarrowSweepWorker(bufferA, bufferB, localResults, range.Item1, range.Item2),
            localResults => { allResults.Add(localResults); });
        foreach (var i in allResults) results.AddRange(i);
    }

    

    
}