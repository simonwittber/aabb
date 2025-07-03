using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AABB.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 3, iterationCount: 3)]
[MemoryDiagnoser]
public class CollisionTesterBenchmark
{
    [Params(CollisionTester.ExecutionMode.Scalar, CollisionTester.ExecutionMode.Threaded, CollisionTester.ExecutionMode.Vectorized, CollisionTester.ExecutionMode.ThreadedVectorized, CollisionTester.ExecutionMode.Auto)]
    public CollisionTester.ExecutionMode ExecutionMode;
    
    [Params(10, 100, 1000, 10000, 100000)]
    public int N = 1000;
    private CollisionLayer bufferA, bufferB;
    private CollisionTester collider;
    public List<(int, int)> results = new();
    [IterationSetup]
    public void Setup()
    {
        bufferA = new CollisionLayer();
        bufferB = new CollisionLayer();
        var rnd = new System.Random(1979);
        float P() => rnd.NextSingle() * 2000;
        float S() => rnd.NextSingle() * 50;
        
        for (int i = 0; i < N/10; i++)
            bufferA.Add(new Box(i, P(), P(), S(), S()));
        for (int i = 0; i < N; i++)
        {
            bufferB.Add(new Box(N+i, P(), P(), S(), S()));
        }
        collider = new CollisionTester();
        results.Clear();
        collider.Collisions(bufferA, bufferB, results);
    }

    [Benchmark(OperationsPerInvoke =1)]
    public void TestCollisions()
    {
        collider.Mode = ExecutionMode;
        results.Clear();
        collider.Collisions(bufferA, bufferB, results);    
    }
    
   
   
}