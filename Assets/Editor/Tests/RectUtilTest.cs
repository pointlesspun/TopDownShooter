using UnityEngine;
using NUnit.Framework;
using Tds.Util;

public class RectUtilTest
{
    [Test]
    public void ShouldReturnFalseWhenCallingDisconnectedOnTheSameRect()
    {
        // trivial test two rects that are the same
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);

        Assert.IsFalse(RectUtil.AreDisconnected(r1, r2));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r1));      
        Assert.IsFalse(RectUtil.AreDisconnected(r2, r2));
    }

    [Test]
    public void ShouldReturnFalseWhenRectsAreTouching()
    {
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.right * 2, Vector2Int.one * 2);
        var r3 = new RectInt(Vector2Int.left * 2, Vector2Int.one * 2);
        var r4 = new RectInt(Vector2Int.up* 2, Vector2Int.one * 2);
        var r5 = new RectInt(Vector2Int.down * 2, Vector2Int.one * 2);

        Assert.IsFalse(RectUtil.AreDisconnected(r1, r2));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r3));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r4));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r5));
    }

    [Test]
    public void ShouldReturnFalseWhenRectsAreHalfTouching()
    {
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.right * 2 + Vector2Int.up, Vector2Int.one * 2);
        var r3 = new RectInt(Vector2Int.left * 2 + Vector2Int.down, Vector2Int.one * 2);
        var r4 = new RectInt(Vector2Int.up * 2 + Vector2Int.right, Vector2Int.one * 2);
        var r5 = new RectInt(Vector2Int.down * 2 + Vector2Int.left, Vector2Int.one * 2);

        Assert.IsFalse(RectUtil.AreDisconnected(r1, r2));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r3));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r4));
        Assert.IsFalse(RectUtil.AreDisconnected(r1, r5));
    }

    [Test]
    public void ShouldReturnTrueWhenEdgesTouch()
    {
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.left * 2 + Vector2Int.up * 2, Vector2Int.one * 2);
        var r3 = new RectInt(Vector2Int.right * 2 + Vector2Int.up * 2, Vector2Int.one * 2);
        var r4 = new RectInt(Vector2Int.left * 2 + Vector2Int.down * 2, Vector2Int.one * 2);
        var r5 = new RectInt(Vector2Int.right * 2 + Vector2Int.down * 2, Vector2Int.one * 2);

        Assert.IsTrue(RectUtil.AreDisconnected(r1, r2));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r3));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r4));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r5));
    }

    [Test]
    public void ShouldReturnTrueApart()
    {
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.left * 3 , Vector2Int.one * 2);
        var r3 = new RectInt(Vector2Int.right * 3 , Vector2Int.one * 2);
        var r4 = new RectInt(Vector2Int.up * 3 , Vector2Int.one * 2);
        var r5 = new RectInt(Vector2Int.down * 3, Vector2Int.one * 2);

        Assert.IsTrue(RectUtil.AreDisconnected(r1, r2));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r3));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r4));
        Assert.IsTrue(RectUtil.AreDisconnected(r1, r5));
    }

    [Test]
    public void TestGetTouchPoints()
    {
        // right 
        var r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        var r2 = new RectInt(Vector2Int.right * 2, Vector2Int.one * 2);
        var touchPoints = RectUtil.GetTouchPoints(r1, r2);

        Assert.IsTrue(touchPoints[0].x == 2);
        Assert.IsTrue(touchPoints[1].x == 2);
        Assert.IsTrue(touchPoints[0].y == 0);
        Assert.IsTrue(touchPoints[1].y == 2);

        // right & down
        r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        r2 = new RectInt(Vector2Int.right * 2 + Vector2Int.down, Vector2Int.one * 2);
        touchPoints = RectUtil.GetTouchPoints(r1, r2);

        Assert.IsTrue(touchPoints[0].x == 2);
        Assert.IsTrue(touchPoints[1].x == 2);
        Assert.IsTrue(touchPoints[0].y == 0);
        Assert.IsTrue(touchPoints[1].y == 1);

        // bottom & left
        r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        r2 = new RectInt(Vector2Int.left + Vector2Int.down * 2, Vector2Int.one * 2);
        touchPoints = RectUtil.GetTouchPoints(r1, r2);

        Assert.IsTrue(touchPoints[0].x == 0);
        Assert.IsTrue(touchPoints[1].x == 1);
        Assert.IsTrue(touchPoints[0].y == 0);
        Assert.IsTrue(touchPoints[1].y == 0);

        // top
        r1 = new RectInt(Vector2Int.zero, Vector2Int.one * 2);
        r2 = new RectInt(Vector2Int.zero + Vector2Int.up * 2, Vector2Int.one * 2);
        touchPoints = RectUtil.GetTouchPoints(r1, r2);

        Assert.IsTrue(touchPoints[0].x == 0);
        Assert.IsTrue(touchPoints[1].x == 2);
        Assert.IsTrue(touchPoints[0].y == 2);
        Assert.IsTrue(touchPoints[1].y == 2);

    }
}
