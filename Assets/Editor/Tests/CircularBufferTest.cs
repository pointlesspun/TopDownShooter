using NUnit.Framework;
using Tds.DungeonGeneration;
using Tds.PathFinder;
using Tds.Util;

public class CircularBufferTest
{
    [Test]
    public void TestEmptyBuffer_ExpectBufferEmpty()
    {
        var buffer = new CircularBuffer<object>(3);
        Assert.IsTrue(buffer.Count == 0);
    }

    [Test]
    public void TestSingleAdd_ExpectBufferToContainOneElement()
    {
        var buffer = new CircularBuffer<object>(3);

        buffer.Add("boo");

        Assert.IsTrue(buffer.Count == 1);
        Assert.IsTrue(buffer[0].ToString() == "boo");
    }

    [Test]
    public void TestOverflow_ExpectBufferToContainMaxElements()
    {
        var buffer = new CircularBuffer<object>(3);
        string[] strings = { "boo", "foo", "baz", "qaz" };

        foreach (var s in strings)
        {
            buffer.Add(s);
        }

        Assert.IsTrue(buffer.Count == 3);

        for (var i = 0; i < buffer.Count; ++i)
        {
            Assert.IsTrue(buffer[i].ToString() == strings[1+i]);
        }

        Assert.IsTrue(buffer.Last.ToString() == strings[3]);
    }

    [Test]
    public void TestOverflowMax_ExpectBufferToContainMaxElements()
    {
        var buffer = new CircularBuffer<object>(3);
        string[] strings = { "boo", "foo", "baz", "qaz" };

        foreach (var s in strings)
        {
            buffer.Add(s);
        }

        Assert.IsTrue(buffer[0].ToString() == "foo");
        Assert.IsTrue(buffer[1].ToString() == "baz");
        Assert.IsTrue(buffer[2].ToString() == "qaz");


        // this will alter the indices in the buffer...
        buffer.__D(int.MaxValue - 1);

        // ... so check what the expectations are
        Assert.IsTrue(buffer[0].ToString() == "qaz");
        Assert.IsTrue(buffer[1].ToString() == "foo");
        Assert.IsTrue(buffer[2].ToString() == "baz");

        buffer.Add("x");

        Assert.IsTrue(buffer[0].ToString() == "foo");
        Assert.IsTrue(buffer[1].ToString() == "baz");
        Assert.IsTrue(buffer[2].ToString() == "x");

        buffer.Add("y");

        Assert.IsTrue(buffer.Count == 3);
        Assert.IsTrue(buffer[0].ToString() == "baz");
        Assert.IsTrue(buffer[1].ToString() == "x");
        Assert.IsTrue(buffer[2].ToString() == "y");
    }
}
