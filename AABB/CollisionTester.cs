
namespace AABB;

public partial class CollisionTester
{
    private List<(int aIndex, int bIndex)> intersectsX = new(37);
    private List<(int aIndex, int bIndex)> intersectsY = new(37);

    public enum ExecutionMode
    {
        Threaded, Vectorized, ThreadedVectorized, Scalar, Auto
    }
    
    public ExecutionMode Mode = ExecutionMode.Auto;
    
    public void Collisions(CollisionLayer layerA, CollisionLayer layerB, List<(int aIndex, int bIndex)> results)
    {
        BroadPhaseCollisions(layerA, layerB);
        NarrowPhaseCollisions(layerA, layerB, results);
    }

    void BroadPhaseCollisions(CollisionLayer layerA, CollisionLayer layerB)
    {
        intersectsX.Clear();
        if (layerA.Count > 0 && layerB.Count > 0)
        {
            Mode = ChooseExecutionMode(layerA.Count);
            switch (Mode)
            {
                case ExecutionMode.ThreadedVectorized:
                    BufferVsBuffer_Threaded_Vectorized(layerA.minX, layerA.maxX, layerA.Count, layerB.minX, layerB.maxX, layerB.Count, intersectsX);
                    break;
                case ExecutionMode.Scalar:
                    BufferVsBuffer_Scalar(layerA.minX, layerA.maxX, layerA.Count, layerB.minX, layerB.maxX, layerB.Count, intersectsX);
                    break;
                case ExecutionMode.Vectorized:
                    BufferVsBuffer_Vectorized(layerA.minX, layerA.maxX, layerA.Count, layerB.minX, layerB.maxX, layerB.Count, intersectsX);
                    break;
                case ExecutionMode.Threaded:
                    BufferVsBuffer_Threaded(layerA.minX, layerA.maxX, layerA.Count, layerB.minX, layerB.maxX, layerB.Count, intersectsX);
                    break;
            }
        }
    }

    private ExecutionMode ChooseExecutionMode(int workload)
    {
        if (Mode == ExecutionMode.Auto)
        {
            // Automatically determine the best architecture based on the workload
            if (workload < 50)
            {
                Mode = ExecutionMode.Scalar;
            }
            else if (workload < 5000)
            {
                Mode = ExecutionMode.Vectorized;
            }
            else
            {
                Mode = ExecutionMode.ThreadedVectorized;
            }
        }
        return Mode;
    }

    void NarrowPhaseCollisions(CollisionLayer layerA, CollisionLayer layerB, List<(int aIndex, int bIndex)> results)
    {
        intersectsY.Clear();
        if (intersectsX.Count <= 0) return;
        Mode = ChooseExecutionMode(intersectsX.Count);
        switch (Mode)
        {
            case ExecutionMode.Threaded:
                NarrowSweep_Threaded(layerA, layerB, intersectsY);
                break;
            case ExecutionMode.Vectorized:
                NarrowSweep_Vectorized(layerA, layerB, intersectsY);
                break;
            case ExecutionMode.ThreadedVectorized:
                NarrowSweep_Vectorized_Threaded(layerA, layerB, intersectsY);
                break;
            case ExecutionMode.Scalar:
                NarrowSweep_Scalar(layerA, layerB, intersectsY);
                break;
        }
        // convert indexes to id
        for (int i = 0; i < intersectsY.Count; i++)
        {
            var (dynamicIndex, staticIndex) = intersectsY[i];
            results.Add((layerA.ids[dynamicIndex], layerB.ids[staticIndex]));
        }
    }
}