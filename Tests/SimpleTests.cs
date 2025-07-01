namespace Tests;

using AABB;

public class SimpleTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestSameDimensions()
    {
        var box1 = new Box(0f, 0f, 5f, 5f);
        var box2 = new Box(0f, 0f, 5f, 5f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }

    [Test]
    public void TestTouching()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(1f, 1f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }
    
    [Test]
    public void TestLeftIntersection()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(-0.5f, 0f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }
    
    [Test]
    public void TestRightIntersection()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(0.5f, 0f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }
    
    [Test]
    public void TestLeftMiss()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(-1.5f, 0f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.False);
    }
    
    [Test]
    public void TestRightMiss()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(1.5f, 0f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.False);
    }
    
    [Test]
    public void TestTopIntersection()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(0f, 0.5f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }
    
    [Test]
    public void TestBottomIntersection()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(0f, -0.5f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.True);
    }
    
    [Test]
    public void TestTopMiss()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(0f, 1.1f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.False);
    }
    
    [Test]
    public void TestBottomMiss()
    {
        var box1 = new Box(0f, 0f, 1f, 1f);
        var box2 = new Box(0f, -1.1f, 1f, 1f);
        Assert.That(Box.Intersects(box1, box2), Is.False);
    }
}