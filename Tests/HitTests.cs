using System.Numerics;
using AABB;

namespace Tests;

public class HitTests
{
    [Test]
    public void HitTestSimple()
    {
        var layer = new CollisionLayer();
        var a = layer.Add(new Box(0, 0, 10, 10));
        var b = layer.Add(new Box(50, 50, 10, 10));

        var hits = new List<int>();
        layer.HitTest(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(a));
        
        hits.Clear();
        layer.HitTest(new Vector2(119, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(0));

        hits.Clear();
        layer.HitTest(new Vector2(51, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(b));
    }
    
    [Test]
    public void HitTestOverlap()
    {
        var layer = new CollisionLayer();
        var a = layer.Add(new Box(3, 3, 10, 10));
        var b = layer.Add(new Box(10, 10, 10, 10));
        var c = layer.Add(new Box(100, 100, 10, 10));

        var hits = new List<int>();
        layer.HitTest(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(a));
        
        hits.Clear();
        layer.HitTest(new Vector2(7, 7), hits);
        Assert.That(hits.Count, Is.EqualTo(2));
        Assert.That(hits, Contains.Item(a));
        Assert.That(hits, Contains.Item(b));
        Assert.That(hits, Does.Not.Contain(c));
    }
    
    [Test]
    public void HitTestSimple_Vectorized()
    {
        var layer = new CollisionLayer();
        var a = layer.Add(new Box(0, 0, 10, 10));
        var b = layer.Add(new Box(50, 50, 10, 10));

        var hits = new List<int>();
        layer.HitTestVectorized(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(a));

        hits.Clear();        
        layer.HitTestVectorized(new Vector2(119, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(0));
        
        hits.Clear();
        layer.HitTestVectorized(new Vector2(51, 51), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(b));
    }
    
    [Test]
    public void HitTestOverlap_Vectorized()
    {
        var layer = new CollisionLayer();
        var a = layer.Add(new Box(3, 3, 10, 10));
        var b = layer.Add(new Box(10, 10, 10, 10));
        var c = layer.Add(new Box(100, 100, 10, 10));
        var hits = new List<int>();
        layer.HitTestVectorized(new Vector2(1, 1), hits);
        Assert.That(hits.Count, Is.EqualTo(1));
        Assert.That(hits[0], Is.EqualTo(a));
        
        hits.Clear();
        layer.HitTestVectorized(new Vector2(7, 7), hits);
        Assert.That(hits.Count, Is.EqualTo(2));
        Assert.That(hits, Contains.Item(a));
        Assert.That(hits, Contains.Item(b));
        Assert.That(hits, Does.Not.Contain(c));
    }
}