using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AABB.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 3, iterationCount: 3)]
[MemoryDiagnoser]
public class HitTestBenchmark
{
    [Params(10, 100, 1000, 10000, 100000)] public int N = 1000;
    private CollisionLayer bufferA;
    private Vector2[] points = new Vector2[100];
    List<int> hits = new List<int>(32);
    [IterationSetup]
    public void Setup()
    {
        bufferA = new CollisionLayer();
        var rnd = new System.Random(1979);
        float P() => rnd.NextSingle() * 2000;
        float S() => rnd.NextSingle() * 50;

        for (int i = 0; i < N; i++)
            bufferA.Add(new Box(i, P(), P(), S(), S()));

        for (int i = 0; i < 100; i++)
            points[i] = new Vector2(rnd.NextSingle(), rnd.NextSingle()) * 2000;
    }

    [Benchmark(OperationsPerInvoke = 100)]
    public void TestHitTest()
    {
        for (var i = 0; i < points.Length; i++)
        {
            hits.Clear();
            var point = points[i];
            bufferA.HitTest(point, hits);
        }
    }
    
    [Benchmark(OperationsPerInvoke = 100)]
    public void TestHitTest_Vectorized()
    {
        for (var i = 0; i < points.Length; i++)
        {
            hits.Clear();
            var point = points[i];
            bufferA.HitTestVectorized(point, hits);
        }
    }
}