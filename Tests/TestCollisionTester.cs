using AABB;

namespace Tests;

[TestFixture]
public class TestCollisionTester
{
    
    [Test]
    public void TestNoCollisions([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };

        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        

        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
            
        bufferA.Add(box1);
        bufferA.Add(box2);
        
        bufferB.Add(new Box(1,5,2,1));
        
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results, Has.Count.EqualTo(0));
    }
    
    [Test]
    public void TestSingleCollision([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        

        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
            
        bufferA.Add(box1);
        bufferA.Add(box2);
        
        bufferB.Add(new Box(0.5f,0,1,1));

        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo((0,0)));
    }

    [Test]
    public void TestMultiCollision([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        
        var bufferA = new CollisionLayer();
        bufferA.Add(new Box(0, 0, 1, 1));
        bufferA.Add(new Box(5, 0, 1, 1));
        bufferA.Add(new Box(1, 0, 1, 1));
        
        var bufferB = new CollisionLayer();
        bufferB.Add(new Box(0.5f,0,1,1));
        
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Console.WriteLine(string.Join(",",results));
        
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results, Contains.Item((0,0)));
        Assert.That(results, Contains.Item((2,0)));
    }
    
    [Test]
    public void TestAllToAllCollisions([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
        for (int i = 0; i < 3; i++)
            bufferA.Add(new Box(i, 0, 1, 1));
        for (int j = 0; j < 3; j++)
            bufferB.Add(new Box(j, 0, 1, 1));
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Console.WriteLine(results);
        Assert.That(results, Has.Count.EqualTo(7));
    }

    [Test]
    public void TestEdgeTouching([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
        bufferA.Add(new Box(0, 0, 1, 1));
        bufferB.Add(new Box(1, 0, 1, 1));
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results, Has.Count.EqualTo(1));
    }

    [Test]
    public void TestNoOverlapNegativeCoords([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
        bufferA.Add(new Box(-10, -10, 2, 2));
        bufferB.Add(new Box(10, 10, 2, 2));
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results, Has.Count.EqualTo(0));
    }

    [Test]
    public void TestLargeNumberOfBoxes([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
        for (int i = 0; i < 100; i++)
            bufferA.Add(new Box(i, 0, 1, 1));
        for (int j = 0; j < 100; j++)
            bufferB.Add(new Box(j, 0, 1, 1));
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results.Count, Is.GreaterThan(0));
    }

    [Test]
    public void TestEmptyLayers([Values] CollisionTester.ExecutionMode executionMode)
    {
        var collider = new CollisionTester() { Mode = executionMode };
        var bufferA = new CollisionLayer();
        var bufferB = new CollisionLayer();
        var results = new List<(int, int)>();
        collider.Collisions(bufferA, bufferB, results);
        Assert.That(results, Has.Count.EqualTo(0));
    }

}