using UnityEngine;
using NUnit.Framework;
using Tds.Util;

public class TestLine2D
{
    [Test]
    public void TestRandomized()
    {
        var line = new Line2D()
        {
            from = Vector2.zero,
            to = Vector2.right * 4
        };

        Random.InitState(42);

        for (int i = 0; i < 100; ++i)
        {
            var r = line.Randomized(1, 8);
            r = r.Snap();

            Assert.IsTrue(r.Length >= 1 && r.Length <= line.to.x - 1);
            Assert.IsTrue(r.from.x < r.to.x);
            Assert.IsTrue(r.from.x >= line.from.x && r.to.x <= line.to.x);
        }
    }

    [Test]
    public void TestRandomizedWithConstraints()
    {
        var line = new Line2D()
        {
            from = Vector2.zero,
            to = Vector2.right * 6
        };

        Random.InitState(42);

        for (int i = 0; i < 100; ++i)
        {
            var r = line.Randomized(1, 8, 1, 1);
            r = r.Snap();

            Assert.IsTrue(r.Length >= 1 && r.Length <= line.to.x - 1);
            Assert.IsTrue(r.from.x < r.to.x);
            Assert.IsTrue(r.from.x >= line.from.x && r.to.x <= line.to.x);
            Assert.IsTrue(r.from.x >= 1);
            Assert.IsTrue(r.to.x <= 7);
        }
    }
}

