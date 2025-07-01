# A really simple collision library for .NET

It is tested, benchmarked, vectorized and multithreaded.

    var collider = new CollisionTester();
    
    var bufferA = new CollisionLayer();
    bufferA.Add(new Box(0, 0, 1, 1));
    bufferA.Add(new Box(5, 0, 1, 1));
    bufferA.Add(new Box(1, 0, 1, 1));
    
    var bufferB = new CollisionLayer();
    bufferB.Add(new Box(0.5f,0,1,1));
    
    var collisions = collider.Collisions(bufferA, bufferB);
    Console.WriteLine(collisions); // (0, 0),(2, 0)

## Benchmarks
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4349/24H2/2024Update/HudsonValley)
AMD Ryzen 7 3700X 3.59GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-RXFIBC : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

Runtime=.NET 9.0  InvocationCount=1  IterationCount=3
LaunchCount=1  UnrollFactor=1  WarmupCount=3

| Method         | ExecutionMode      | N      | Mean            | Error           | StdDev         | Median          | Op/s         | Allocated |
|--------------- |------------------- |------- |----------------:|----------------:|---------------:|----------------:|-------------:|----------:|
| TestCollisions | Threaded           | 10     |     23,866.7 ns |     91,242.9 ns |     5,001.3 ns |     21,600.0 ns |    41,899.44 |    2288 B |
| TestCollisions | Threaded           | 100    |     32,100.0 ns |     37,254.7 ns |     2,042.1 ns |     31,400.0 ns |    31,152.65 |    6168 B |
| TestCollisions | Threaded           | 1000   |    123,600.0 ns |    148,627.6 ns |     8,146.8 ns |    120,700.0 ns |     8,090.61 |   24616 B |
| TestCollisions | Threaded           | 10000  |  1,726,266.7 ns | 12,930,481.1 ns |   708,763.3 ns |  1,740,900.0 ns |       579.28 |  803072 B |
| TestCollisions | Threaded           | 100000 | 22,417,033.3 ns | 11,696,172.8 ns |   641,106.7 ns | 22,433,400.0 ns |        44.61 |   17664 B |
| TestCollisions | Vectorized         | 10     |        900.0 ns |      4,826.8 ns |       264.6 ns |      1,000.0 ns | 1,111,111.11 |         - |
| TestCollisions | Vectorized         | 100    |      4,333.3 ns |      3,797.7 ns |       208.2 ns |      4,400.0 ns |   230,769.23 |         - |
| TestCollisions | Vectorized         | 1000   |     78,433.3 ns |    120,330.2 ns |     6,595.7 ns |     76,000.0 ns |    12,749.68 |         - |
| TestCollisions | Vectorized         | 10000  |    970,533.3 ns |    662,366.1 ns |    36,306.5 ns |    956,300.0 ns |     1,030.36 |         - |
| TestCollisions | Vectorized         | 100000 | 82,227,433.3 ns | 48,725,472.2 ns | 2,670,807.6 ns | 82,430,700.0 ns |        12.16 |         - |
| TestCollisions | ThreadedVectorized | 10     |     14,700.0 ns |     23,716.8 ns |     1,300.0 ns |     14,000.0 ns |    68,027.21 |    1736 B |
| TestCollisions | ThreadedVectorized | 100    |     66,233.3 ns |    854,352.4 ns |    46,829.9 ns |     40,000.0 ns |    15,098.14 |    5520 B |
| TestCollisions | ThreadedVectorized | 1000   |    120,933.3 ns |    793,050.9 ns |    43,469.8 ns |     97,300.0 ns |     8,269.02 |   16368 B |
| TestCollisions | ThreadedVectorized | 10000  |  1,149,266.7 ns |  1,239,655.7 ns |    67,949.7 ns |  1,126,400.0 ns |       870.12 |  143336 B |
| TestCollisions | ThreadedVectorized | 100000 | 20,082,166.7 ns |  8,247,015.5 ns |   452,046.7 ns | 20,282,300.0 ns |        49.80 |   15200 B |
| TestCollisions | Scalar             | 10     |        566.7 ns |      3,797.7 ns |       208.2 ns |        500.0 ns | 1,764,705.88 |         - |
| TestCollisions | Scalar             | 100    |      2,700.0 ns |      9,479.7 ns |       519.6 ns |      2,400.0 ns |   370,370.37 |         - |
| TestCollisions | Scalar             | 1000   |     90,333.3 ns |    150,633.2 ns |     8,256.7 ns |     95,000.0 ns |    11,070.11 |         - |
| TestCollisions | Scalar             | 10000  |  1,326,033.3 ns |  5,144,039.5 ns |   281,962.2 ns |  1,170,800.0 ns |       754.13 |         - |
| TestCollisions | Scalar             | 100000 | 96,021,766.7 ns | 29,075,876.7 ns | 1,593,746.9 ns | 95,923,000.0 ns |        10.41 |         - |
| TestCollisions | Auto               | 10     |        566.7 ns |      3,797.7 ns |       208.2 ns |        500.0 ns | 1,764,705.88 |         - |
| TestCollisions | Auto               | 100    |      2,433.3 ns |      2,786.8 ns |       152.8 ns |      2,400.0 ns |   410,958.90 |         - |
| TestCollisions | Auto               | 1000   |     74,033.3 ns |     13,813.9 ns |       757.2 ns |     73,700.0 ns |    13,507.43 |         - |
| TestCollisions | Auto               | 10000  |  1,142,266.7 ns |  6,816,555.5 ns |   373,638.4 ns |    928,700.0 ns |       875.45 |         - |
| TestCollisions | Auto               | 100000 | 21,240,666.7 ns | 24,719,135.6 ns | 1,354,939.2 ns | 20,481,000.0 ns |        47.08 |   14984 B |
