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
