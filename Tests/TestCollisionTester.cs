using AABB;

namespace Tests;

[TestFixture]
public class TestCollision
{
    [Test]
    public void TestBroadPhaseCollision()
    {
        var box1 = new Box(0, 0, 1, 1); 
        var box2 = new Box(5, 0, 1, 1);
        var box3 = new Box(0.5f, 0, 1, 1);
        var box4 = new Box(0.5f, 10, 1, 1);

        var broadPhase = new Collision();
        var a = broadPhase.Add(box1);
        var b = broadPhase.Add(box2);
        var c = broadPhase.Add(box3);
        var d = broadPhase.Add(box4);

        var results = broadPhase.BroadPhaseCollisions();

        Assert.That(results, Has.Count.EqualTo(3), "Expected three candidate collisions");
    }

    [Test]
    public void TestNarrowPhaseCollision()
    {
        var box1 = new Box(0, 0, 1, 1);
        var box2 = new Box(5, 0, 1, 1);
        var box3 = new Box(0.5f, 0, 1, 1);
        var box4 = new Box(0.5f, 10, 1, 1);

        var collider = new Collision();
        var a = collider.Add(box1);
        var b = collider.Add(box2);
        var c = collider.Add(box3);
        var d = collider.Add(box4);


        collider.BroadPhaseCollisions();
        var results = collider.NarrowPhaseCollisions();

        Assert.That(results, Has.Count.EqualTo(1), "Expected one actual collision");
        Assert.That(results[0], Is.EqualTo((a,c)), "Expected collision with box1 and box3");
    }
}