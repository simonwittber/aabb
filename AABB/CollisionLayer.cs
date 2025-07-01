using System.Numerics;

namespace AABB;

public class CollisionLayer
{
    private int[] idMap = new int[37];

    internal int[] ids = new int[37];
    internal float[] minX = new float[37];
    internal float[] minY = new float[37];
    internal float[] maxX = new float[37];
    internal float[] maxY = new float[37];

    public int Count { get; private set; }

    public Box[] ToArray()
    {
        var result = new Box[Count];
        for (var i = 0; i < Count; i++)
        {
            result[i] = CreateBox(i);
        }

        return result;
    }

    private void Resize()
    {
        var requiredSize = Count + 1; // Increment size for new box
        if (requiredSize <= 0) return;
        if (requiredSize < ids.Length) return;
        requiredSize = Math.Max(requiredSize, ids.Length * 2);
        Array.Resize(ref ids, requiredSize);
        Array.Resize(ref idMap, requiredSize);
        Array.Resize(ref minX, requiredSize);
        Array.Resize(ref minY, requiredSize);
        Array.Resize(ref maxX, requiredSize);
        Array.Resize(ref maxY, requiredSize);
    }

    void Set(int index, float xMin, float yMin, float xMax, float yMax)
    {
        minX[index] = xMin;
        minY[index] = yMin;
        maxX[index] = xMax;
        maxY[index] = yMax;
    }

    void Swap(int index, int otherIndex)
    {
        (minX[index], minX[otherIndex]) = (minX[otherIndex], minX[index]);
        (minY[index], minY[otherIndex]) = (minY[otherIndex], minY[index]);
        (maxX[index], maxX[otherIndex]) = (maxX[otherIndex], maxX[index]);
        (maxY[index], maxY[otherIndex]) = (maxY[otherIndex], maxY[index]);
        var idA = ids[index];
        var idB = ids[otherIndex];
        (ids[index], ids[otherIndex]) = (ids[otherIndex], ids[index]);
        idMap[idA] = otherIndex;
        idMap[idB] = index;
    }

    public int Add(Box box)
    {
        Resize();
        // find index to insert using binary search on minX
        // find first index in minX where minX[index] >= box.xMin
        var index = FindInsertIndex(box);
        // move all elements from index to the right by one
        if (index < Count)
        {
            int moveCount = Count - index;
            Array.Copy(minX, index, minX, index + 1, moveCount);
            Array.Copy(minY, index, minY, index + 1, moveCount);
            Array.Copy(maxX, index, maxX, index + 1, moveCount);
            Array.Copy(maxY, index, maxY, index + 1, moveCount);
            Array.Copy(ids, index, ids, index + 1, moveCount);

            for (int i = index + 1; i <= Count; i++)
            {
                idMap[ids[i]] = i;
            }
        }

        // Set the new box at the found index
        Set(index, box.xMin, box.yMin, box.xMax, box.yMax);
        // Assign the ID to the new box
        var id = Count;
        ids[index] = id;
        idMap[id] = index;
        Count++;
        return id;
    }

    private int FindInsertIndex(Box box)
    {
        var index = Array.BinarySearch(minX, 0, Count, box.xMin);
        if (index < 0)
            index = ~index;
        return index;
    }

    public void Update(int id, Box box)
    {
        var index = idMap[id];
        minX[index] = box.xMin;
        minY[index] = box.yMin;
        maxX[index] = box.xMax;
        maxY[index] = box.yMax;
        // if this X is smaller than the previous one, we need to move it left
        if (index > 0 && minX[index] < minX[index - 1])
        {
            ShuffleLeft(index);
        }
        // if this X is larger than the next one, we need to move it right
        else if (index < Count - 1 && minX[index] > minX[index + 1])
        {
            ShuffleRight(index);
        }
    }

    private void ShuffleRight(int index)
    {
        while (index < Count - 1 && minX[index] > minX[index + 1])
        {
            Swap(index, index + 1);
            index++;
        }
    }

    private void ShuffleLeft(int index)
    {
        while (index > 0 && minX[index] < minX[index - 1])
        {
            Swap(index, index - 1);
            index--;
        }
    }

    public Box Get(int id)
    {
        var index = idMap[id];
        return CreateBox(index);
    }

    private Box CreateBox(int index)
    {
        var min = new Vector2(minX[index], minY[index]);
        var max = new Vector2(maxX[index], maxY[index]);
        var box = new Box
        {
            center = 0.5f * (min + max),
            extents = 0.5f * (max - min),
        };
        return box;
    }

    public void Remove(int id)
    {
        var indexToRemove = idMap[id];
        var lastIndex = Count - 1;
        if (indexToRemove < lastIndex)
        {
            Swap(indexToRemove, lastIndex);
            idMap[ids[indexToRemove]] = indexToRemove;
            Count--;
            idMap[ids[lastIndex]] = -1; // Remove the last index from the map
        }
        else if (indexToRemove == lastIndex)
        {
            Count--;
            idMap[ids[lastIndex]] = -1; // Remove the last index from the map
        }
        else
        {
            throw new IndexOutOfRangeException($"Box ID {id} is out of bounds for buffer of length {Count}.");
        }
    }

    public Span<int>.Enumerator GetEnumerator()
    {
        return ids.AsSpan(0, Count).GetEnumerator();
    }
}