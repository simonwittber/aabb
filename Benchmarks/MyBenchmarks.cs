using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AABB.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 3, iterationCount: 3)]
[MemoryDiagnoser]
public class MyBenchmarks
{
    [Params(10, 100, 1000, 10000, 100000)]
    public int N = 1000;
    private BoxBuffer bufferA, bufferB;
    private CollisionTester collider;
    [IterationSetup]
    public void Setup()
    {
        bufferA = new BoxBuffer();
        bufferB = new BoxBuffer();
        var rnd = new System.Random(1979);
        float P() => rnd.NextSingle() * 1000;
        float S() => rnd.NextSingle() * 50;
        
        for (int i = 0; i < N/10; i++)
            bufferA.Add(new Box(P(), P(), S(), S()));
        for (int i = 0; i < N; i++)
        {
            bufferB.Add(new Box(P(), P(), S(), S()));
        }
        collider = new CollisionTester();
        collider.Collisions(bufferA, bufferB);
    }

    [Benchmark(OperationsPerInvoke =1)]
    public void TestCollisions_Scalar()
    {
        collider.architecture = CollisionTester.Architecture.Scalar;
        collider.Collisions(bufferA, bufferB);    
    }
    
    [Benchmark(OperationsPerInvoke =1)]
    public void TestCollisions_Threaded()
    {
        collider.architecture = CollisionTester.Architecture.Threaded;
        collider.Collisions(bufferA, bufferB);    
    }
    
    [Benchmark(OperationsPerInvoke =1)]
    public void TestCollisions_Vectorized()
    {
        collider.architecture = CollisionTester.Architecture.Vectorized;
        collider.Collisions(bufferA, bufferB);    
    }
    
    [Benchmark(OperationsPerInvoke =1)]
    public void TestCollisions_TheadedVectorized()
    {
        collider.architecture = CollisionTester.Architecture.ThreadedVectorized;
        collider.Collisions(bufferA, bufferB);    
    }
   
}