using AABB;

namespace Tests;

[TestFixture]
public class TestCollisionTester
{
    
    [Test]
    public void TestNoCollisions([Values] CollisionTester.Architecture architecture)
    {
        var collider = new CollisionTester() { architecture = architecture };

        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        

        var bufferA = new BoxBuffer();
        var bufferB = new BoxBuffer();
            
        bufferA.Add(box1);
        bufferA.Add(box2);
        
        bufferB.Add(new Box(1,5,2,1));
        
        var narrowPhaseCollisions = collider.Collisions(bufferA, bufferB);
        Assert.That(narrowPhaseCollisions, Has.Count.EqualTo(0));
    }
    
    [Test]
    public void TestSingleCollision([Values] CollisionTester.Architecture architecture)
    {
        var collider = new CollisionTester() { architecture = architecture };
        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        

        var bufferA = new BoxBuffer();
        var bufferB = new BoxBuffer();
            
        bufferA.Add(box1);
        bufferA.Add(box2);
        
        bufferB.Add(new Box(0.5f,0,1,1));
        
        var narrowPhaseCollisions = collider.Collisions(bufferA, bufferB);
        Assert.That(narrowPhaseCollisions, Has.Count.EqualTo(1));
        Assert.That(narrowPhaseCollisions[0], Is.EqualTo((0,0)));
    }

    [Test]
    public void TestMultiCollision([Values] CollisionTester.Architecture architecture)
    {
        var collider = new CollisionTester() { architecture = architecture };
        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        var box3 = new Box(1, 0, 1, 1);

        
        var bufferA = new BoxBuffer();
        var bufferB = new BoxBuffer();
            
        bufferA.Add(box1);
        bufferA.Add(box2);
        bufferA.Add(box3);
        
        bufferB.Add(new Box(0.5f,0,1,1));
        
        var narrowPhaseCollisions = collider.Collisions(bufferA, bufferB);
        Assert.That(narrowPhaseCollisions, Has.Count.EqualTo(2));
        Assert.That(narrowPhaseCollisions, Contains.Item((0,0)));
        Assert.That(narrowPhaseCollisions, Contains.Item((2,0)));
    }
    
}