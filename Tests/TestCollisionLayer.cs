using System.Numerics;
using AABB;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class TestCollisionLayer
{
    [Test]
    public void TestAddIncreasesCount()
    {
        var layer = new CollisionLayer();
        Assert.That(layer.Count, Is.EqualTo(0));
        layer.Add(new Box(0, 0, 0, 1, 1));
        Assert.That(layer.Count, Is.EqualTo(1));
        layer.Add(new Box(1, 1, 1, 2, 2));
        Assert.That(layer.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestUpdateChangesBox()
    {
        var layer = new CollisionLayer();
        var box = new Box(0, 0, 0, 1, 1);
        layer.Add(box);
        box.center = new Vector2(1, 1);
        layer.Update(box);
        var newBox = layer.Get(box.id);
        Assert.That(newBox, Is.EqualTo(box));
    }

    [Test]
    public void TestRemoveDecreasesCount()
    {
        var layer = new CollisionLayer();
        layer.Add(new Box(0, 0, 0, 1, 1));
        layer.Add(new Box(1, 1, 1, 2, 2));
        Assert.That(layer.Count, Is.EqualTo(2));
        layer.Remove(0);
        Assert.That(layer.Count, Is.EqualTo(1));
        layer.Remove(1);
        Assert.That(layer.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestRemoveInvalidIdThrows()
    {
        var layer = new CollisionLayer();
        Assert.Throws<KeyNotFoundException>(() => layer.Remove(0));
    }

    [Test]
    public void TestOrder()
    {
        int[] xValues = [1,56,-2,3,7,1,2,4,2,-3,1,5,7,9,8,6,4,-3,2,1];
        var layer = new CollisionLayer();
        for (int i = 0; i < xValues.Length; i++)
        {
            layer.Add(new Box(i, xValues[i], 0, 1, 1));
        }
        var boxes = layer.ToArray();
        for (int i = 0; i < boxes.Length - 1; i++)
        {
            Assert.That(boxes[i].xMin, Is.LessThanOrEqualTo(boxes[i + 1].xMin));
        }
    }
    
    [Test]
    public void TestIdIsMaintained()
    {
        int[] xValues = [1,56,-2,3,7,1,2,4,2,-3,1,5,7,9,8,6,4,-3,2,1];
        var layer = new CollisionLayer();
        var map = new Dictionary<int, Box>();
        int id = 0;
        foreach (var x in xValues)
        {
            var box = new Box(id, x, 0, x*2, 1);
            layer.Add(box);
            map[id] = box;
            id += 1;
        }
        foreach (var b in layer)
        {
            Assert.That(map[b], Is.EqualTo(layer.Get(b)));
        }
        
    }
}

