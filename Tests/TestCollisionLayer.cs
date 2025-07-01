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
        layer.Add(new Box(0, 0, 1, 1));
        Assert.That(layer.Count, Is.EqualTo(1));
        layer.Add(new Box(1, 1, 2, 2));
        Assert.That(layer.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestUpdateChangesBox()
    {
        var layer = new CollisionLayer();
        var id = layer.Add(new Box(0, 0, 1, 1));
        layer.Update(id, new Box(2, 2, 3, 3));
        var newBox = layer.Get(id);
        Assert.That(newBox, Is.EqualTo(new Box(2, 2, 3, 3)));
    }

    [Test]
    public void TestRemoveDecreasesCount()
    {
        var layer = new CollisionLayer();
        var id1 = layer.Add(new Box(0, 0, 1, 1));
        var id2 = layer.Add(new Box(1, 1, 2, 2));
        Assert.That(layer.Count, Is.EqualTo(2));
        layer.Remove(id1);
        Assert.That(layer.Count, Is.EqualTo(1));
        layer.Remove(id2);
        Assert.That(layer.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestRemoveInvalidIdThrows()
    {
        var layer = new CollisionLayer();
        Assert.Throws<IndexOutOfRangeException>(() => layer.Remove(0));
    }

    [Test]
    public void TestOrder()
    {
        int[] xValues = [1,56,-2,3,7,1,2,4,2,-3,1,5,7,9,8,6,4,-3,2,1];
        var layer = new CollisionLayer();
        for (int i = 0; i < xValues.Length; i++)
        {
            var id = layer.Add(new Box(xValues[i], 0, 1, 1));
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
        foreach (var x in xValues)
        {
            var box = new Box(x, 0, x*2, 1);
            var id = layer.Add(box);
            map[id] = box;
        }
        foreach (var b in layer)
        {
            Assert.That(map[b], Is.EqualTo(layer.Get(b)));
        }
        
    }
}

