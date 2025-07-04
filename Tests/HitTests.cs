using System.Numerics;
using AABB;

namespace Tests;

public class HitTests
{
    [Test]
    public void HitTestSimple()
    {
        var layer = new CollisionLayer();
        layer.Add(new Box(202, 0, 0, 10, 10));
        layer.Add(new Box(303, 50, 50, 10, 10));

        var hits = new List<int>();
        layer.HitTest(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(202));
        
        hits.Clear();
        layer.HitTest(new Vector2(119, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(0));

        hits.Clear();
        layer.HitTest(new Vector2(51, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(303));
    }
    
    [Test]
    public void HitTestOverlap()
    {
        var layer = new CollisionLayer();
        layer.Add(new Box(0, 3, 3, 10, 10));
        layer.Add(new Box(1, 10, 10, 10, 10));
        layer.Add(new Box(2, 100, 100, 10, 10));

        var hits = new List<int>();
        layer.HitTest(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(0));
        
        hits.Clear();
        layer.HitTest(new Vector2(7, 7), hits);
        Assert.That(hits.Count, Is.EqualTo(2));
        Assert.That(hits, Contains.Item(0));
        Assert.That(hits, Contains.Item(1));
        Assert.That(hits, Does.Not.Contain(2));
    }
    
    [Test]
    public void HitTestSimple_Vectorized()
    {
        var layer = new CollisionLayer();
        layer.Add(new Box(0, 0, 0, 10, 10));
        layer.Add(new Box(1, 50, 50, 10, 10));

        var hits = new List<int>();
        layer.HitTestVectorized(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(0));

        hits.Clear();        
        layer.HitTestVectorized(new Vector2(119, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(0));
        
        hits.Clear();
        layer.HitTestVectorized(new Vector2(51, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(1));
    }
    
    [Test]
    public void HitTestOverlap_Vectorized()
    {
        var layer = new CollisionLayer();
        layer.Add(new Box(0, 3, 3, 10, 10));
        layer.Add(new Box(1, 10, 10, 10, 10));
        layer.Add(new Box(2, 100, 100, 10, 10));
        var hits = new List<int>();
        layer.HitTestVectorized(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(0));
        
        hits.Clear();
        layer.HitTestVectorized(new Vector2(7, 7), hits);
        Assert.That(hits.Count, Is.EqualTo(2));
        Assert.That(hits, Contains.Item(0));
        Assert.That(hits, Contains.Item(1));
        Assert.That(hits, Does.Not.Contain(2));
    }

    [Test]
    public void HitTestWithSize()
    {
        var layer = new CollisionLayer();
        
        layer.Add(new Box(99, 0, 0, 10, 10));
        layer.Add(new Box(77, 50, 50, 10, 10));
        
        var hits = new List<int>();
        layer.HitTest(new Vector2(-6, -6), new Vector2(3,3), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(99));
        
        hits.Clear();
        layer.HitTest(new Vector2(-6, -6), new Vector2(1,1), hits);
        Assert.That(hits.Count, Is.EqualTo(0));
        
        hits.Clear();
        layer.HitTest(new Vector2(-6, -6), new Vector2(2,2), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(99));

    }
}