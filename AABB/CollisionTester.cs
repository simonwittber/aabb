using System.Collections.Concurrent;
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

public partial class CollisionTester
{
    public List<(int aIndex, int bIndex)> intersectsX = new(37);
    public List<(int aIndex, int bIndex)> intersectsY = new(37);

    public enum Architecture
    {
        Threaded, Vectorized, ThreadedVectorized, Scalar
    }
    
    public Architecture architecture = Architecture.ThreadedVectorized;
    
    public List<(int aIndex, int bIndex)> Collisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        BroadPhaseCollisions(bufferA, bufferB);
        return NarrowPhaseCollisions(bufferA, bufferB);
    }

    void BroadPhaseCollisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        intersectsX.Clear();
        if (bufferA.Count > 0 && bufferB.Count > 0)
        {
            switch (architecture)
            {
                case Architecture.ThreadedVectorized:
                    BufferVsBuffer_Threaded_Vectorized(bufferA.centerX, bufferA.extentX, bufferA.Count, bufferB.centerX, bufferB.extentX, bufferB.Count, intersectsX);
                    break;
                case Architecture.Scalar:
                    BufferVsBuffer_Scalar(bufferA.centerX, bufferA.extentX, bufferA.Count, bufferB.centerX, bufferB.extentX, bufferB.Count, intersectsX);
                    break;
                case Architecture.Vectorized:
                    BufferVsBuffer_Vectorized(bufferA.centerX, bufferA.extentX, bufferA.Count, bufferB.centerX, bufferB.extentX, bufferB.Count, intersectsX);
                    break;
                case Architecture.Threaded:
                    BufferVsBuffer_Threaded(bufferA.centerX, bufferA.extentX, bufferA.Count, bufferB.centerX, bufferB.extentX, bufferB.Count, intersectsX);
                    break;
            }
            
        }
    }

    List<(int aIndex, int bIndex)> NarrowPhaseCollisions(BoxBuffer bufferA, BoxBuffer bufferB)
    {
        intersectsY.Clear();
        if (intersectsX.Count <= 0) return intersectsY;
        switch (architecture)
        {
            case Architecture.Threaded:
                NarrowSweep_Threaded(bufferA, bufferB, intersectsY);
                break;
            case Architecture.Vectorized:
                NarrowSweep_Vectorized(bufferA, bufferB, intersectsY);
                break;
            case Architecture.ThreadedVectorized:
                NarrowSweep_Vectorized_Threaded(bufferA, bufferB, intersectsY);
                break;
            case Architecture.Scalar:
                NarrowSweep_Scalar(bufferA, bufferB, intersectsY);
                break;
        }
        // convert indexes to id
        for (int i = 0; i < intersectsY.Count; i++)
        {
            var (dynamicIndex, staticIndex) = intersectsY[i];
            intersectsY[i] = (bufferA.ids[dynamicIndex], bufferB.ids[staticIndex]);
        }
        return intersectsY;
    }
}