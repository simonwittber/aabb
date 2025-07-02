# A really simple collision library for .NET

It is tested, benchmarked, vectorized and multithreaded.

    var collider = new CollisionTester();
    
    var bufferA = new CollisionLayer();
    var idA = bufferA.Add(new Box(0, 0, 1, 1));
    var idB = bufferA.Add(new Box(5, 0, 1, 1));
    var idC = bufferA.Add(new Box(1, 0, 1, 1));
    
    var bufferB = new CollisionLayer();
    var idD = bufferB.Add(new Box(0.5f,0,1,1));
    
    var collisions = new List<(int indexA, int indexB)>();
    collider.Collisions(bufferA, bufferB, collisions);

    // collisions a list of tuples, each tuple is an id from the first
    // layer, and and id from the second layer where a collision is detected.
    Console.WriteLine(collisions); // [(0, 0),(2, 0)]

    var hits = new List<int>();
    bufferB.HitTest(new Vector2(0,0), hits);

    // hits is a list of id values from the buffer where a hit was detected.
    Console.WriteLine(hits); // [0]



