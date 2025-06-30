using System.Numerics;

namespace AABB;

public class BoxBuffer
{
    internal int[] ids = new int[37];
    internal int[] idMap = new int[37];
    internal float[] centerX = new float[37];
    internal float[] centerY = new float[37];
    internal float[] extentX = new float[37];
    internal float[] extentY = new float[37];
    
    public int Count { get; private set; }

    private void Resize()
    {
        var size = Count + 1; // Increment size for new box
        if (size <= 0) return;
        if (size < ids.Length) return;
        size = Math.Max(size, size * 2);
        Array.Resize(ref ids, size);
        Array.Resize(ref idMap, size);
        Array.Resize(ref centerX, size);
        Array.Resize(ref centerY, size);
        Array.Resize(ref extentX, size);
        Array.Resize(ref extentY, size);
    }

    public int Add(Box box)
    {
        Resize();
        centerX[Count] = box.center.X;
        centerY[Count] = box.center.Y;
        extentX[Count] = box.extents.X;
        extentY[Count] = box.extents.Y;
        var boxId = Count;
        ids[Count] = boxId; // Store the index as the ID
        idMap[boxId] = boxId; // Map the ID to itself
        Count++;
        return boxId;
    }

    public void Update(int id, Box box)
    {
        centerX[id] = box.center.X;
        centerY[id] = box.center.Y;
        extentX[id] = box.extents.X;
        extentY[id] = box.extents.Y;
    }

    public void Remove(int id)
    {
        var indexToRemove = idMap[id];
        var lastIndex = Count - 1;
        if (indexToRemove < lastIndex)
        {
            // Move the last box to the removed position
            centerX[indexToRemove] = centerX[lastIndex];
            centerY[indexToRemove] = centerY[lastIndex];
            extentX[indexToRemove] = extentX[lastIndex];
            extentY[indexToRemove] = extentY[lastIndex];
            ids[indexToRemove] = ids[lastIndex]; // Update ID to match the moved box
            idMap[ids[indexToRemove]] = indexToRemove; // Update the ID map
            Count--;
        }
        else if (indexToRemove == lastIndex)
        {
            // Just remove the last box
            Count--;
        }
        else
        {
            throw new IndexOutOfRangeException($"Box ID {id} is out of bounds for buffer of length {Count}.");
        }
    }
}

public class CollisionTester
{
    public List<(int aIndex, int bIndex)> intersectsX = new(37);
    public List<(int aIndex, int bIndex)> intersectsY = new(37);
    
    public List<(int aIndex, int bIndex)> Collisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        BroadPhaseCollisions(bufferA, bufferB);
        return NarrowPhaseCollisions(bufferA, bufferB);
    }

    void BroadPhaseCollisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        intersectsX.Clear();
        BufferVsBuffer(bufferA.centerX, bufferA.extentX, bufferA.Count, bufferB.centerX, bufferB.extentX, bufferB.Count, intersectsX);
    }

    List<(int aIndex, int bIndex)> NarrowPhaseCollisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        intersectsY.Clear();
        NarrowSweep(bufferA, bufferB);
        // convert indexes to id
        for (int i = 0; i < intersectsY.Count; i++)
        {
            var (dynamicIndex, staticIndex) = intersectsY[i];
            intersectsY[i] = (bufferA.ids[dynamicIndex], bufferB.ids[staticIndex]);
        }
        return intersectsY;
    }

    private void NarrowSweep(BoxBuffer bufferA, BoxBuffer bufferB)
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
                    intersectsY.Add(intersectsX[idx + k]);

            idx += W;
        }

        // scalar remainder
        for (; idx < n; idx++)
        {
            var (dynamicIndex, staticIndex) = intersectsX[idx];
            if (bufferA.centerY[dynamicIndex] - bufferA.extentY[dynamicIndex] <= bufferB.centerY[staticIndex] + bufferB.extentY[staticIndex] && bufferA.centerY[dynamicIndex] + bufferA.extentY[dynamicIndex] >= bufferB.centerY[staticIndex] - bufferB.extentY[staticIndex])
            {
                intersectsY.Add((dynamicIndex, staticIndex));
            }
        }
    }

    private static void BufferVsBuffer(
        ReadOnlySpan<float> aCenters, ReadOnlySpan<float> aExtents, int aCount,
        ReadOnlySpan<float> bCenters, ReadOnlySpan<float> bExtents, int bCount, List<(int aIndex, int bIndex)> results)
    {
        var W = Vector<float>.Count;

        for (var i = 0; i < aCount; i++)
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
                var testCenter = new Vector<float>(bCenters.Slice(j, W));
                var testExtent = new Vector<float>(bExtents.Slice(j, W));
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
}